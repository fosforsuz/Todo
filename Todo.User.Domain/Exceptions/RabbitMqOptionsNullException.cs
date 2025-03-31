namespace Todo.User.Domain.Exceptions;

public class RabbitMqOptionsNullException : BaseException
{
    public RabbitMqOptionsNullException() : base("RabbitMQ options are null")
    {
    }
}