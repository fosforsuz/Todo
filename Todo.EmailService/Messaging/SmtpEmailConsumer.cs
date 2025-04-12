using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Todo.EmailService.Service.Abstraction;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;

namespace Todo.EmailService.Messaging;

public class SmtpEmailConsumer : BackgroundService
{
    private readonly ILogger<SmtpEmailConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly RabbitMqConfig _rabbitMqConfig;

    public SmtpEmailConsumer(ILogger<SmtpEmailConsumer> logger, IEmailService emailService,
        IOptions<RabbitMqConfig> rabbitMqConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _rabbitMqConfig = rabbitMqConfig.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqConfig.HostName,
            Port = _rabbitMqConfig.Port,
            UserName = _rabbitMqConfig.UserName,
            Password = _rabbitMqConfig.Password,
            AutomaticRecoveryEnabled = _rabbitMqConfig.AutomaticRecoveryEnabled,
            ClientProvidedName = _rabbitMqConfig.ConnectionName
        };

        await using var connection =
            await connectionFactory.CreateConnectionAsync(cancellationToken: stoppingToken);

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqQueues.EmailQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel: channel);
        consumer.ReceivedAsync += ProcessSendingEmailMessage(stoppingToken: stoppingToken);

        await channel.BasicConsumeAsync(
            queue: RabbitMqQueues.EmailQueue,
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private AsyncEventHandler<BasicDeliverEventArgs> ProcessSendingEmailMessage(
        CancellationToken stoppingToken)
    {
        return async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var emailEvent = JsonConvert.DeserializeObject<EmailEvent>(message);

            if (emailEvent is null)
            {
                _logger.LogError("Failed to deserialize email event");
                return;
            }

            if (emailEvent.RetryCount > _rabbitMqConfig.RetryCount)
            {
                _logger.LogWarning("Retry count exceeded for email event: {EmailEvent}", emailEvent.ToJson());

                if (emailEvent.DoesMetaDataMatchKeyValue("FromDlq", "true"))
                {
                    _logger.LogWarning("Email event retry count exceeded, sending to DLQ: {EmailEvent}",
                        emailEvent.ToJson());
                    // Handle DLQ logic here
                    return;
                }
                
                
            }

            try
            {
                _emailService.SendEmailAsync(emailEvent, stoppingToken);
                _logger.LogInformation("Email sent successfully, Email Event: ", emailEvent.ToJson());
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        };
    }
}