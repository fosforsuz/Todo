using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Todo.Shared.Abstraction;

namespace Todo.Shared.Infrastructure;

internal class Repository<T> : IRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet;

    protected Repository(DbContext dbContext)
    {
        var context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(bool tracking = false, CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate, bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetAsync<TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector, bool tracking = false, CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).Where(predicate).Select(selector)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> GetAsync<TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector, int skip, int take, bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).Where(predicate).Select(selector)
            .Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, bool tracking = false,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> GetSingleAsync<TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector, bool tracking = false, CancellationToken cancellationToken = default)
    {
        return await GetQueryable(tracking).Where(predicate).Select(selector).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<T?> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(keyValues, cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var addedEntity = await _dbSet.AddAsync(entity, cancellationToken);
        addedEntity.State = EntityState.Added;
        return addedEntity.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var updatedEntity = _dbSet.Update(entity);
        updatedEntity.State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        var removedEntity = _dbSet.Remove(entity);
        removedEntity.State = EntityState.Deleted;
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return GetQueryable().CountAsync(predicate, cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return GetQueryable().AnyAsync(predicate, cancellationToken);
    }

    public Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return GetQueryable().AllAsync(predicate, cancellationToken);
    }

    private IQueryable<T> GetQueryable(bool tracking = false)
    {
        return tracking ? _dbSet : _dbSet.AsNoTracking();
    }
}