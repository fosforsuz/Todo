using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdateUserCommand : IdentifiableCommand
{
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public bool IsNotificationEnabled { get; set; }
}
