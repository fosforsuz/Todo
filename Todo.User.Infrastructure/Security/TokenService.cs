using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Shared.Contracts.Config;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Models;

namespace Todo.User.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtTokenConfig _config;

    public TokenService(IOptions<JwtTokenConfig> options)
    {
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, string username, string role,
        string? refreshToken = null, DateTime? refreshTokenExpires = null)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.Secret));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };


        var datetimeNow = DateTime.UtcNow;
        var expires = datetimeNow.AddMinutes(_config.ExpirationInMinutes);


        var jwt = new JwtSecurityToken(
            _config.Issuer,
            _config.Audience,
            claims,
            datetimeNow,
            expires,
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return Task.FromResult(new TokenResponse
        {
            Token = token ?? string.Empty,
            Expires = expires,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTokenExpires
        });
    }
}