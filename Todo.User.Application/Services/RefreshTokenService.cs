using Microsoft.Extensions.Options;
using Todo.Shared.Contracts.Config;
using Todo.SharedKernel.Abstraction;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Utils;
using Todo.User.Domain.Entity;
using Todo.User.Infrastructure.Abstraction;

namespace Todo.User.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtTokenConfig _config;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenService(IUnitOfWork unitOfWork, IOptions<JwtTokenConfig> options)
    {
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _refreshTokenRepository = unitOfWork.GetCustomRepository<IRefreshTokenRepository>() ??
                                  throw new ArgumentNullException(nameof(unitOfWork));
    }


    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(refreshToken))
            throw new ArgumentNullException(nameof(refreshToken));

        var refreshTokenEntity = await _refreshTokenRepository.GetSingleAsync(
            predicate: token => token.Token == refreshToken && !token.IsRevoked && !token.IsUsed,
            cancellationToken: cancellationToken
        );

        return refreshTokenEntity;
    }

    public async Task<RefreshToken> CreateRefreshToken(Guid userId, string? ipAddress,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            throw new ArgumentNullException(nameof(userId));

        var token = GenerateService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationInDays);

        var refreshToken =
            RefreshToken.Create(userId, token, expiresAt, ipAddress);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return refreshToken;
    }

    public async Task MarkRefreshTokenAsUsedAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        refreshToken.MarkAsUsed();
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        refreshToken.MarkAsRevoked();
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
    }
}