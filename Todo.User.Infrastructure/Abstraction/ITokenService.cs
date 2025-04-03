using Todo.User.Infrastructure.Models;

namespace Todo.User.Infrastructure.Abstraction;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, string username, string role,
        string? refreshToken = null, DateTime? refreshTokenExpires = null);
}