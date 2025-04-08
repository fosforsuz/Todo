using MediatR;
using Todo.SharedKernel.Enums;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdateUserRoleCommand : IdentifiableCommand, IRequest<Result<CommandResponse>>
{
    public Role Role { get; set; }
}