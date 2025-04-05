using Todo.SharedKernel.Events;
using Todo.SharedKernel.Messaging;

namespace Todo.User.Infrastructure.Messaging;

public class RabbitMqEmailPublisher : IRabbitMqEmailPublisher
{
    public Task PublishEmailEventAsync(EmailEvent emailEvent, string queueName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}