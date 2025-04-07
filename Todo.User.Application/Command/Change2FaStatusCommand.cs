using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class Change2FaStatusCommand : IdentifiableCommand, IRequest<Result<CommandResponse>>
{
    public bool Is2FaEnabled { get; init; }
}