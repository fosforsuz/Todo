namespace Todo.Shared.Exceptions;

public class TransactionAlreadyStartedException : Exception
{
    public TransactionAlreadyStartedException() : base("Transaction already started.")
    {
    }
}