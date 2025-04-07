using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class SendVerifyMailCommand : IRequest<Result<CommandResponse>>
{
    public Guid UserId { get; set; }
}