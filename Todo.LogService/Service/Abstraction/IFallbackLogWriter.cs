using Todo.SharedKernel.Events;

namespace Todo.LogService.Service.Abstraction;

public interface IFallbackLogWriter
{
    void Write(LogEvent logEvent, string? message);
}