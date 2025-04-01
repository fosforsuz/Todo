namespace Todo.User.Application.Command;

public class LoginCommand
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}