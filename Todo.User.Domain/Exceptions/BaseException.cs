namespace Todo.User.Domain.Exceptions;

public class BaseException : Exception
{
    private Guid Id { get; set; }
    private string? ErrorCode { get; set; }

    public BaseException() : base("An error occurred.")
    {
    }

    public BaseException(string message) : base(message)
    {
    }

    public BaseException(string message, Exception innerException, string? errorCode) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public BaseException(Guid id) : base($"An error occurred with ID: {id}")
    {
        Id = id;
    }

    public BaseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public BaseException(string message, params object[] args) : base(string.Format(message, args))
    {
    }
}