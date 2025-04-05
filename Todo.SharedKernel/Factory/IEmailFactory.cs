using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Events;

namespace Todo.SharedKernel.Factory;

public interface IEmailFactory
{
    Task<EmailEvent> CreateAsync(EmailEventType type, string to, Dictionary<string, string?> metadata);
}