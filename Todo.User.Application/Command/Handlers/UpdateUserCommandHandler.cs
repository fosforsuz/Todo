using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<CommandResponse>>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<CommandResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserAsync(request, cancellationToken);
    }
}