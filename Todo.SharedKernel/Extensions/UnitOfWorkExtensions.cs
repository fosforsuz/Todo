using Todo.SharedKernel.Abstraction;

namespace Todo.SharedKernel.Extensions;

public static class UnitOfWorkExtensions
{
    public static async Task SafeRollbackAsync(this IUnitOfWork unitOfWork, CancellationToken ct)
    {
        if (unitOfWork.IsTransactionStarted)
            await unitOfWork.RollbackTransactionAsync(ct);
    }
}