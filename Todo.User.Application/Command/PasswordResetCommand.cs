using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class PasswordResetCommand : IRequest<Result<CommandResponse>>
{
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}