using Microsoft.Extensions.Options;
using Todo.Shared.Abstraction;
using Todo.Shared.Config;
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
}