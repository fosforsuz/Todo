using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command;

namespace Todo.User.Application.Abstraction;

public interface IUserService
{
    Task<Result<CommandResponse>> RegisterUserAsync(RegisterCommand registerCommand,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken);

    Task<Result<CommandResponse>> UpdatePasswordAsync(UpdatePasswordCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> UpdateUserRoleAsync(UpdateUserRoleCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken);
}