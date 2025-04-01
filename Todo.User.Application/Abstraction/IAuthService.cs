using Todo.Shared.Results;
using Todo.User.Application.Command;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Application.Abstraction;

public interface IAuthService
{
    Task<Result<TokenResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
}