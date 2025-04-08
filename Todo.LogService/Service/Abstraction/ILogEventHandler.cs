using Todo.SharedKernel.Events;

namespace Todo.LogService.Service.Abstraction;

public interface ILogEventHandler
{
    Task HandleAsync(LogEvent logEvent, CancellationToken cancellationToken = default);
}