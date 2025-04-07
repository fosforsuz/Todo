using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class Change2FaStatusCommand : IdentifiableCommand
{
    public bool Is2FaEnabled { get; init; }
}