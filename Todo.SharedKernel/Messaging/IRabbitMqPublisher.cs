using Todo.SharedKernel.Events;

namespace Todo.SharedKernel.Messaging;

public interface IRabbitMqEmailPublisher
{
    Task PublishEmailEventAsync(EmailEvent emailEvent, string queueName, CancellationToken cancellationToken);
}