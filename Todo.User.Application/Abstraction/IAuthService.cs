using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Abstraction;

public interface IAuthService
{
    Task<Result<TokenResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken);

    Task<Result<TokenResponse>> VerifyOtpAsync(VerifyOtpCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> SendVerifyMailAsync(SendVerifyMailCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> VerifyEmailAsync(VerifyMailCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> SendPasswordResetMailAsync(SendPasswordResetMailCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> PasswordResetAsync(PasswordResetCommand command,
        CancellationToken cancellationToken);

    Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> Change2FaStatusAsync(Change2FaStatusCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken);
}