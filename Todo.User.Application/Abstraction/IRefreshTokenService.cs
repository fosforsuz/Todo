using Todo.User.Domain.Entity;

namespace Todo.User.Application.Abstraction;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshToken(Guid userId, string? ipAddress,
        CancellationToken cancellationToken);
}