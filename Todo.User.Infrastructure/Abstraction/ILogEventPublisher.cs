using Todo.SharedKernel.Events;

namespace Todo.User.Infrastructure.Abstraction;

public interface ILogEventPublisher
{
    Task PublishAsync(LogEvent logEvent, CancellationToken cancellationToken = default);
}