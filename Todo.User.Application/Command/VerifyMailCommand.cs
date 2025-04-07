namespace Todo.User.Application.Command;

public class VerifyMailCommand
{
    public string VerifyToken { get; set; } = string.Empty;
}