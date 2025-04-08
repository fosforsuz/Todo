using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Todo.LogService.Service.Abstraction;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Service;

public class LogEventPublisher : ILogEventPublisher
{
    private readonly RabbitMqConfig _config;

    public LogEventPublisher(IConfiguration configuration)
    {
        var rabbitMqConfig = new RabbitMqConfig();
        configuration.GetSection(nameof(RabbitMqConfig)).Bind(rabbitMqConfig);
        _config = rabbitMqConfig;
    }

    public async Task PublishAsync(LogEvent logEvent, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            RabbitMqQueues.LogEventQueue,
            true,
            false,
            false,
            cancellationToken: cancellationToken
        );

        var properties = new BasicProperties
        {
            Persistent = true,
            DeliveryMode = DeliveryModes.Persistent
        };

        var jsonMessage = JsonConvert.SerializeObject(logEvent);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        await channel.BasicPublishAsync(
            string.Empty,
            RabbitMqQueues.LogEventQueue,
            false,
            properties,
            body,
            cancellationToken
        );
    }
}