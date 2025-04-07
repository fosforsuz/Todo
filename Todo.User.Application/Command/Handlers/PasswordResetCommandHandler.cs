using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, Result<CommandResponse>>
{
    private readonly IAuthService _authService;

    public PasswordResetCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CommandResponse>> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        return await _authService.PasswordResetAsync(request, cancellationToken);
    }
}