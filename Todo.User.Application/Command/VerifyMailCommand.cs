using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;

namespace Todo.User.Application.Command;

public class VerifyMailCommand : IRequest<Result<CommandResponse>>
{
    public string VerifyToken { get; set; } = string.Empty;
}