using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Command;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Abstraction;

public interface IAuthService
{
    Task<Result<TokenResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken);

    Task<Result<CommandResponse>> SendVerifyMailAsync(SendVerifyMailCommand command,
        CancellationToken cancellationToken);

    Task<Result<CommandResponse>> VerifyEmailAsync(VerifyMailCommand command,
        CancellationToken cancellationToken);
    
}