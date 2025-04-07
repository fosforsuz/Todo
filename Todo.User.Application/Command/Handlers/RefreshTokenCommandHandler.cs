using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Command.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshTokenAsync(request, cancellationToken);
    }
}