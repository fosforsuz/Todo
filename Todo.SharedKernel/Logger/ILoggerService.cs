namespace Todo.SharedKernel.Logger;

public interface ILoggerService<T> where T : class
{
    Task LogInformationAsync(string message, CancellationToken cancellationToken = default);
    Task LogWarningAsync(string message, CancellationToken cancellationToken = default);
    Task LogDebugAsync(string message, CancellationToken cancellationToken = default);
    Task LogErrorAsync(string message, Exception exception, CancellationToken cancellationToken = default);
    Task LogCriticalAsync(string message, Exception exception, CancellationToken cancellationToken = default);

    Task LogByExceptionSeverityAsync(string contextMessage, Exception exception,
        CancellationToken cancellationToken = default);
}