using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Infrastructure.Messaging;

public class RabbitMqLogEventPublisher : ILogEventPublisher
{
    private readonly LogRabbitMqConfig _config;

    public RabbitMqLogEventPublisher(IOptions<LogRabbitMqConfig> config)
    {
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
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