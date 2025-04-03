namespace Todo.SharedKernel.Exceptions;

public class TransactionNotStartedException : Exception
{
    public TransactionNotStartedException() : base("Transaction has not been started.")
    {
    }
}