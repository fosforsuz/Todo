namespace Todo.SharedKernel.Events;

public class LogEvent : DomainEvent
{
    public required string Level { get; set; }
    public string Message { get; set; } = null!;
    public string? ExceptionMessage { get; set; }
    public string? ExceptionType { get; set; }
    public string? StackTrace { get; set; }

    public string Source { get; set; } = null!;
    public Dictionary<string, string>? Metadata { get; set; }

    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
    public string? CorrelationId { get; set; }
}