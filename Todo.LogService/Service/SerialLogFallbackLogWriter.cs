using Serilog;
using Todo.LogService.Service.Abstraction;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Service;

public class SerilogFallbackLogWriter : IFallbackLogWriter
{
    public void Write(LogEvent logEvent, string? message)
    {
        if (message == null)
            Log.Logger.Error("Fallback log: {Data}", logEvent.ToJson());
        else
            Log.Logger.Error("Fallback log with message: {Message}, Data: {Data}", message, logEvent.ToJson());
    }
}