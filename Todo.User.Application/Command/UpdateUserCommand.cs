using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdateUserCommand : IdentifiableCommand, IRequest<Result<CommandResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsNotificationEnabled { get; set; }
}