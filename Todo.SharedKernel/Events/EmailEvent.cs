using Todo.SharedKernel.Enums;

namespace Todo.SharedKernel.Events;

public class EmailEvent : DomainEvent
{
    public required string To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? HtmlBody { get; set; }
    public string? From { get; set; }
    public EmailEventType Type { get; set; }
    public Dictionary<string, string?>? Metadata { get; set; }
}