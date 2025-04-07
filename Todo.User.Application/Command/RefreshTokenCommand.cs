namespace Todo.User.Application.Command;

public class RefreshTokenCommand
{
    public required string Token { get; set; }
    public string? IpAddress { get; set; }
}