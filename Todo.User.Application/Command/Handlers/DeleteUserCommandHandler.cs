using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<CommandResponse>>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<CommandResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.DeleteUserAsync(request, cancellationToken);
    }
}