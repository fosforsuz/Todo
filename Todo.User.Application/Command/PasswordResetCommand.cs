using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class PasswordResetCommand
{
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}
