using System;

namespace Todo.SharedKernel.Exceptions;

public class SaveChangesFailedException : Exception
{
    public SaveChangesFailedException() : base("Failed to save changes to the database.")
    {
    }

    public SaveChangesFailedException(string message) : base(message)
    {
    }

    public SaveChangesFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}