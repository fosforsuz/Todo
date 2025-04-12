namespace Todo.EmailService.Config;

public class SmtpEmailConfig
{
    public string SmtpServer { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; }
}