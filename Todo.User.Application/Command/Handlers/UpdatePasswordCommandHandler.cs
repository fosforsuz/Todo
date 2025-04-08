using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<CommandResponse>>
{
    private readonly IUserService _userService;

    public UpdatePasswordCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<CommandResponse>> Handle(UpdatePasswordCommand request,
        CancellationToken cancellationToken)
    {
        return await _userService.UpdatePasswordAsync(request, cancellationToken);
    }
}