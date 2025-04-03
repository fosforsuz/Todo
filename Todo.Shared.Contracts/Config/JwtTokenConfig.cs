namespace Todo.Shared.Contracts.Config;

public class JwtTokenConfig
{
    public required string Audience { get; set; }
    public required string Issuer { get; set; }
    public required string Secret { get; set; }
    public int ExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
}