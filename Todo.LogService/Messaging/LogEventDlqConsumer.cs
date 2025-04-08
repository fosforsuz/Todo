using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Todo.LogService.Service.Abstraction;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Messaging;

public class LogEventDlqConsumer : BackgroundService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly ILogEventPublisher _publisher;
    private readonly ILogger<LogEventDlqConsumer> _logger;


    public LogEventDlqConsumer(IConfiguration configuration, ILogger<LogEventDlqConsumer> logger,
        ILogEventPublisher publisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher;
        var rabbitMqConfig = new RabbitMqConfig();
        configuration.GetSection(nameof(RabbitMqConfig)).Bind(rabbitMqConfig);
        _rabbitMqConfig = rabbitMqConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqConfig.HostName,
            Port = _rabbitMqConfig.Port,
            UserName = _rabbitMqConfig.UserName,
            Password = _rabbitMqConfig.Password,
            AutomaticRecoveryEnabled = _rabbitMqConfig.AutomaticRecoveryEnabled,
            ClientProvidedName = _rabbitMqConfig.ConnectionName
        };

        await using var connection =
            await factory.CreateConnectionAsync(_rabbitMqConfig.ConnectionName, cancellationToken: stoppingToken);

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: RabbitMqQueues.LogEventDlqQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += ProcessLoggingConsumerOnReceivedAsync(stoppingToken);

        await channel.BasicConsumeAsync(
            queue: RabbitMqQueues.LogEventDlqQueue,
            autoAck: true, // For logging, we can set this to true because not critical information
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private AsyncEventHandler<BasicDeliverEventArgs> ProcessLoggingConsumerOnReceivedAsync(
        CancellationToken stoppingToken)
    {
        return async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var logEvent = JsonSerializer.Deserialize<LogEvent>(message);


            if (logEvent == null)
            {
                _logger.LogError("Failed to deserialize log event");
                return;
            }

            logEvent.ResetRetryCount();
            logEvent.AddMetaData("FromDlq", "true");


            try
            {
                await _publisher.PublishAsync(logEvent, RabbitMqQueues.LogEventQueue, stoppingToken);
                _logger.LogInformation("Log event: {Parameter}", logEvent.ToJson());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log event");
            }
        };
    }
}