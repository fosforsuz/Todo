using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<CommandResponse>>
{
    private readonly IUserService _userService;

    public RegisterCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<Result<CommandResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _userService.RegisterUserAsync(request, cancellationToken);
    }
}