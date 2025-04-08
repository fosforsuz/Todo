using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command.Abstraction;

namespace Todo.User.Application.Command;

public class UpdatePasswordCommand : IdentifiableCommand, IRequest<Result<CommandResponse>>
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}