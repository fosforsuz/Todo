namespace Todo.Shared.Exceptions;

public class TransactionNotStartedException : Exception
{
    public TransactionNotStartedException() : base("Transaction has not been started.")
    {
    }
}