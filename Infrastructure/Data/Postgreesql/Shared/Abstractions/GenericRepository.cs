using System.Linq.Expressions;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<TEntity>
    where TEntity : Entity
{
    protected readonly TILTContext _context;

    internal DbSet<TEntity> dbSet;

    protected GenericRepository(TILTContext context)
    {
        _context = context;
        this.dbSet = _context.Set<TEntity>();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsyncSourced()
    {
        return await this.dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await this.dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await this.dbSet.FindAsync(id);
    }

    public virtual async Task<TEntity> SaveAsync(TEntity entity)
    {
        await this.dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
        return entity;
    }

    public async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return await this.dbSet.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate)
    {
        return await this.dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }
}


