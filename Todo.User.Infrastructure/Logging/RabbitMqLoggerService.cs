using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Events;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Logger;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Infrastructure.Logging;

public class RabbitMqLoggerService<T> : ILoggerService<T> where T : class
{
    private readonly ILogEventPublisher _publisher;

    public RabbitMqLoggerService(ILogEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task LogInformationAsync(string message, CancellationToken cancellationToken = default)
    {
        return SendLogAsync(ErrorLevel.Info, message, null, cancellationToken);
    }

    public Task LogWarningAsync(string message, CancellationToken cancellationToken = default)
    {
        return SendLogAsync(ErrorLevel.Low, message, null, cancellationToken);
    }

    public Task LogDebugAsync(string message, CancellationToken cancellationToken = default)
    {
        return SendLogAsync(ErrorLevel.Medium, message, null, cancellationToken);
    }

    public Task LogErrorAsync(string message, Exception exception, CancellationToken cancellationToken = default)
    {
        return SendLogAsync(ErrorLevel.Critical, message, exception, cancellationToken);
    }

    public Task LogCriticalAsync(string message, Exception exception, CancellationToken cancellationToken = default)
    {
        return SendLogAsync(ErrorLevel.Fatal, message, exception, cancellationToken);
    }

    private async Task SendLogAsync(ErrorLevel level, string message, Exception? exception,
        CancellationToken cancellationToken)
    {
        var logEvent = new LogEvent
        {
            Level = level.GetErrorLevel(),
            Message = message,
            Exception = exception?.Message,
            StackTrace = exception?.StackTrace,
            Context = typeof(T).Name,
            ContextData = null
        };

        await _publisher.PublishAsync(logEvent, cancellationToken);
    }
}