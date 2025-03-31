namespace Todo.User.Domain.Exceptions;

public class ConnectionStringNullException : BaseException
{
    public ConnectionStringNullException() : base("Connection string is null")
    {
    }
}