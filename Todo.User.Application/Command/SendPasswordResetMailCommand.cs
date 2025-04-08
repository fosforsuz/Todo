using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class SendPasswordResetMailCommand : IRequest<Result<CommandResponse>>
{
    public string Email { get; set; } = string.Empty;
}