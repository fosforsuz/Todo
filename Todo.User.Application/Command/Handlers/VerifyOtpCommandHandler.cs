using MediatR;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Command.Handlers;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<TokenResponse>>
{
    private readonly IAuthService _authService;

    public VerifyOtpCommandHandler(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<TokenResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        return await _authService.VerifyOtpAsync(request, cancellationToken);
    }
}