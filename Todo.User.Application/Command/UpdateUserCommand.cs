using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdateUserCommand : IdentifiableCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsNotificationEnabled { get; set; }
}
