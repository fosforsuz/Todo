using System.Diagnostics;
using System.Net.Mail;
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

    public Task LogInformationAsync(string message, CancellationToken cancellationToken = default) =>
        SendLogAsync(ErrorLevel.Information, message, null, cancellationToken);

    public Task LogWarningAsync(string message, CancellationToken cancellationToken = default) =>
        SendLogAsync(ErrorLevel.Warning, message, null, cancellationToken);

    public Task LogDebugAsync(string message, CancellationToken cancellationToken = default) =>
        SendLogAsync(ErrorLevel.Debug, message, null, cancellationToken);

    public Task LogErrorAsync(string message, Exception exception, CancellationToken cancellationToken = default) =>
        SendLogAsync(ErrorLevel.Error, message, exception, cancellationToken);

    public Task LogCriticalAsync(string message, Exception exception, CancellationToken cancellationToken = default) =>
        SendLogAsync(ErrorLevel.Fatal, message, exception, cancellationToken);

    public async Task LogByExceptionSeverityAsync(
        string contextMessage,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        var level = exception switch
        {
            SmtpException => ErrorLevel.Warning,
            TimeoutException => ErrorLevel.Warning,
            TaskCanceledException => ErrorLevel.Warning,

            ArgumentNullException => ErrorLevel.Error,
            ArgumentException => ErrorLevel.Error,
            InvalidOperationException => ErrorLevel.Error,

            _ => ErrorLevel.Fatal
        };

        await SendLogAsync(level, contextMessage, exception, cancellationToken);
    }


    private async Task SendLogAsync(ErrorLevel level, string message, Exception? exception,
        CancellationToken cancellationToken)
    {
        var logEvent = new LogEvent
        {
            Level = level.ToString(),
            Message = message,
            ExceptionMessage = exception?.Message,
            ExceptionType = exception?.GetType().FullName,
            StackTrace = exception?.StackTrace,
            Source = typeof(T).FullName ?? typeof(T).Name,
            Metadata = RabbitMqLoggerService<T>.BuildDefaultMetadata(),
            TraceId = GetTraceId(),
            SpanId = GetSpanId(),
            CorrelationId = GetCorrelationId()
        };

        await _publisher.PublishAsync(logEvent, cancellationToken);
    }

    private static Dictionary<string, string> BuildDefaultMetadata()
    {
        return new Dictionary<string, string>
        {
            { "MachineName", Environment.MachineName },
            { "AppName", AppDomain.CurrentDomain.FriendlyName },
            { "User", Environment.UserName }
        };
    }

    private static string? GetTraceId() => Activity.Current?.TraceId.ToString();
    private static string? GetSpanId() => Activity.Current?.SpanId.ToString();

    private static string? GetCorrelationId() =>
        Activity.Current?.Baggage.FirstOrDefault(kvp => kvp.Key == "CorrelationId").Value;
}