using Todo.User.Domain.Entity;

namespace Todo.User.Application.Abstraction;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshToken(Guid userId, string? ipAddress,
        CancellationToken cancellationToken);

    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task MarkRefreshTokenAsUsedAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}