namespace Todo.Shared.Abstraction;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IRepository<T> GetRepository<T>() where T : class;
    TRepository GetCustomRepository<TRepository>() where TRepository : class;

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}