namespace Todo.User.Application.Command;

public class SendPasswordResetMailCommand
{
    public string Email { get; set; } = string.Empty;
}