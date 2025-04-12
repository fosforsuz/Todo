using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Todo.Shared.Contracts.Config;
using Todo.Shared.Contracts.Constant;
using Todo.SharedKernel.Events;
using Todo.SharedKernel.Messaging;

namespace Todo.User.Infrastructure.Messaging;

public class RabbitMqEmailPublisher : IRabbitMqEmailPublisher
{

    private readonly RabbitMqConfig _config;

    public RabbitMqEmailPublisher(IOptions<RabbitMqConfig> config)
    {
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task PublishEmailEventAsync(EmailEvent emailEvent, CancellationToken cancellationToken)
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
            RabbitMqQueues.EmailQueue,
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

        var jsonMessage = System.Text.Json.JsonSerializer.Serialize(emailEvent);
        var body = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
        await channel.BasicPublishAsync(
            string.Empty,
            RabbitMqQueues.EmailQueue,
            false,
            properties,
            body,
            cancellationToken
        );
        await channel.CloseAsync(cancellationToken);
        await connection.CloseAsync(cancellationToken);
        await Task.CompletedTask;
    }
}