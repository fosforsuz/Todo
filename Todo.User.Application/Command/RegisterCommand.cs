using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class RegisterCommand : IRequest<Result<CommandResponse>>
{
    public string Name { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PasswordConfirmation { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public required string Role { get; init; } = "Standard";
    public int UtcOffset { get; init; } = 0;
}