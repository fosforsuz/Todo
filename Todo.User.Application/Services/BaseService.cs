using Todo.SharedKernel.Abstraction;
using Todo.SharedKernel.Extensions;
using Todo.SharedKernel.Logger;
using Todo.SharedKernel.Response;
using Todo.SharedKernel.Results;
using Todo.User.Application.Abstraction;

namespace Todo.User.Application.Services;

public abstract class BaseService<T> where T : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerService<T> _logger;

    protected BaseService(IUnitOfWork unitOfWork, ILoggerService<T> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected async Task<Result<CommandResponse>> ExecuteCommandAsync<TCommand>(
        TCommand command,
        Func<Task<Result<CommandResponse>>> action,
        Func<TCommand, Result<CommandResponse>, Task>? onFailure = null,
        Func<TCommand, Result<CommandResponse>, Task>? onSuccess = null,
        Func<TCommand, Exception, Task>? onError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await action();

            if (!result.IsSuccess)
            {
                await _unitOfWork.SafeRollbackAsync(cancellationToken);

                if (onFailure is not null)
                    await onFailure(command, result);

                return result;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            if (onSuccess is not null)
                await onSuccess(command, result);

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.SafeRollbackAsync(cancellationToken);

            if (onError is not null)
                await onError(command, ex);
            else
                await _logger.LogErrorAsync(
                    $"Error executing command of type {typeof(TCommand).Name}: {ex.Message}",
                    ex, cancellationToken);

            return Result<CommandResponse>.Fail("An unexpected error occurred while processing the command.");
        }
    }

    protected static CommandResponse Success(DateTime processedDate, string? location = null,
        Guid? correlationId = null) =>
        new(processedDate, location, correlationId);


    protected async Task SaveAndCommitAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
    }
}