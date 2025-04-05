namespace Todo.User.Application.Command;

public class VerifyMailCommand
{
    public required string VerifyToken { get; set; }
}