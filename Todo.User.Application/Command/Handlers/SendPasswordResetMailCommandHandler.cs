using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class
    SendPasswordResetMailCommandHandler : IRequestHandler<SendPasswordResetMailCommand, Result<CommandResponse>>
{
    private readonly IAuthService _authService;

    public SendPasswordResetMailCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CommandResponse>> Handle(SendPasswordResetMailCommand request,
        CancellationToken cancellationToken)
    {
        return await _authService.SendPasswordResetMailAsync(request, cancellationToken);
    }
}