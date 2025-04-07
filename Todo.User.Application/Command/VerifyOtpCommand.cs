namespace Todo.User.Application.Command;

public class VerifyOtpCommand
{
    public Guid UserId { get; set; }
    public string Otp { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}