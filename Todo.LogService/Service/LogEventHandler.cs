using Todo.LogService.Service.Abstraction;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Service;

public class LogEventHandler : ILogEventHandler
{
    public Task HandleAsync(LogEvent logEvent, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}