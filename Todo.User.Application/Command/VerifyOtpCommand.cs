namespace Todo.User.Application.Command;

public class VerifyOtpCommand
{
    public Guid UserId { get; set; }
    public required string Otp { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}