using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class PasswordResetCommand
{
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
    public required string ConfirmPassword { get; init; }
}
