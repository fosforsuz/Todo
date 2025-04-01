namespace Todo.User.Infrastructure.Models;

public class TokenResponse
{
    public bool IsTwoFactorEnabled { get; set; }
    public required string Token { get; set; }
    public required DateTime Expires { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
    public string? TokenType { get; set; }
}