using Todo.SharedKernel.Events;

namespace Todo.LogService.Service.Abstraction;

public interface ILogEventPublisher
{
    Task PublishAsync(LogEvent logEvent, CancellationToken cancellationToken = default);
}