using Elysium.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Elysium.Infrastructure.Repositories;

public class Repository<T>(DbContext context) : IRepository<T> where T : class
{

    readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await context.AddAsync<T>(entity, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // AsNoTracking method : asks the efcore not to track the entity collection , this saves memory
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id , cancellationToken);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    // TODO : solve the update method problem
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}
