using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class LogoutCommand : IRequest<Result<CommandResponse>>
{
    public string RefreshToken { get; init; } = string.Empty;
}