using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Todo.Shared.Abstraction;
using Todo.Shared.Exceptions;

namespace Todo.User.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _customRepositories = new();
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(UserDbContext userDbContext, IServiceProvider serviceProvider)
    {
        _context = userDbContext ?? throw new ArgumentNullException(nameof(userDbContext));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }


    public IRepository<T> GetRepository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T),
            _ => _serviceProvider.GetRequiredService<IRepository<T>>());
    }

    public TRepository GetCustomRepository<TRepository>() where TRepository : class
    {
        return (TRepository)_customRepositories.GetOrAdd(typeof(TRepository),
            _ => _serviceProvider.GetRequiredService<TRepository>());
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new TransactionNotStartedException();

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();

        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new TransactionNotStartedException();

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();

        _transaction = null;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new TransactionNotStartedException();

        return await _context.SaveChangesAsync(cancellationToken);
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            await _context.DisposeAsync();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}