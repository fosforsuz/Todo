using Todo.SharedKernel.Events;

namespace Todo.LogService.Service.Abstraction;

public interface ILogEventPublisher
{
    Task PublishAsync(LogEvent logEvent, string queue, CancellationToken cancellationToken = default);
}