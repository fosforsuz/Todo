using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Todo.LogService.Service.Abstraction;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Messaging;

public class LogEventConsumer : BackgroundService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly ILogEventHandler _logEventHandler;
    private readonly ILogger<LogEventConsumer> _logger;
    private readonly ILogEventPublisher _publisher;
    private readonly IFallbackLogWriter _fallbackLogWriter;

    public LogEventConsumer(IConfiguration configuration, ILogger<LogEventConsumer> logger,
        ILogEventHandler logEventHandler, ILogEventPublisher publisher, IFallbackLogWriter fallbackLogWriter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logEventHandler = logEventHandler ?? throw new ArgumentNullException(nameof(logEventHandler));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _fallbackLogWriter = fallbackLogWriter ?? throw new ArgumentNullException(nameof(fallbackLogWriter));
        var rabbitMqConfig = new RabbitMqConfig();
        configuration.GetSection(nameof(RabbitMqConfig)).Bind(rabbitMqConfig);
        _rabbitMqConfig = rabbitMqConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
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
            queue: RabbitMqQueues.LogEventQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken
        );

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += ProcessLoggingConsumerOnReceivedAsync(stoppingToken);

        await channel.BasicConsumeAsync(
            queue: RabbitMqQueues.LogEventQueue,
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

            if (logEvent.RetryCount > _rabbitMqConfig.RetryCount)
            {
                _logger.LogWarning("Log event retry count exceeded: {Parameter}", logEvent.ToJson());

                if (logEvent.DoesMetaDataMatchKeyValue("FromDlq", "true"))
                {
                    _fallbackLogWriter.Write(logEvent: logEvent, message: "Log event retry count exceeded");
                    return;
                }

                if (!logEvent.Level.Equals("Error", StringComparison.OrdinalIgnoreCase) &&
                    !logEvent.Level.Equals("Critical", StringComparison.OrdinalIgnoreCase) &&
                    !logEvent.Level.Equals("Fatal", StringComparison.OrdinalIgnoreCase))
                    return;

                await _publisher.PublishAsync(logEvent, RabbitMqQueues.LogEventDlqQueue, stoppingToken);
                return;
            }

            try
            {
                await _logEventHandler.HandleAsync(logEvent, stoppingToken);
                _logger.LogInformation("Log event: {Parameter}", logEvent.ToJson());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log event");
            }
        };
    }
}