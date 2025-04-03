namespace Todo.SharedKernel.Exceptions;

public class TransactionAlreadyStartedException : Exception
{
    public TransactionAlreadyStartedException() : base("Transaction already started.")
    {
    }
}