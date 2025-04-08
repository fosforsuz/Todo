using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Result<CommandResponse>>
{
    private readonly IUserService _userService;

    public UpdateUserRoleCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<CommandResponse>> Handle(UpdateUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserRoleAsync(request, cancellationToken);
    }
}