using MediatR;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Command.Handlers;

public class VerifyMailCommandHandler : IRequestHandler<VerifyMailCommand, Result<CommandResponse>>
{
    private readonly IAuthService _authService;

    public VerifyMailCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CommandResponse>> Handle(VerifyMailCommand request, CancellationToken cancellationToken)
    {
        return await _authService.VerifyEmailAsync(request, cancellationToken);
    }
}