using Todo.SharedKernel.Enums;

namespace Todo.User.Application.Command;

public class RegisterCommand
{
    public required string Name { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string PasswordConfirmation { get; init; }
    public string? Phone { get; init; }
    public required string Role { get; init; } = "Standard";
    public int UtcOffset { get; init; } = 0;
}
