using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OPS.WebApi.Data.Interface;

namespace OPS.WebApi.Data.Repositories;

public class EfRepository<T>(AppDbContext dbContext) : IRepository<T>
    where T : class, IEntity
{
    protected readonly AppDbContext DbContext = dbContext;

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>()
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => DbContext.SaveChangesAsync(cancellationToken);
}