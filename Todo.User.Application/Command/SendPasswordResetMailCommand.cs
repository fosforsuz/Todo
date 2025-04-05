namespace Todo.User.Application.Command;

public class SendPasswordResetMailCommand
{
    public required string Email { get; set; }
}