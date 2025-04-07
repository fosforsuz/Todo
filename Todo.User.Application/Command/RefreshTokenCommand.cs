namespace Todo.User.Application.Command;

public class RefreshTokenCommand
{
    public string Token { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
}