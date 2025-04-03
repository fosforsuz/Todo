namespace Todo.SharedKernel.Events;

public class LogEvent : DomainEvent
{
    public string Level { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Context { get; set; }
    public string? ContextData { get; set; }
}