using System.Linq.Expressions;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<TEntity>
    where TEntity : Entity
{
    protected readonly TILTContext _context;

    internal DbSet<TEntity> Entities;

    protected GenericRepository(TILTContext context)
    {
        _context = context;
        this.Entities = _context.Set<TEntity>();
    }

    #region METHODS

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsyncSourced()
    {
        return await this.Entities.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await this.Entities.AsNoTracking().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await this.Entities.FindAsync(id);
    }

    public virtual async Task<TEntity> SaveAsync(TEntity entity)
    {
        await this.Entities.AddAsync(entity);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.SetUpdated();
        Entities.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
        return entity;
    }

    public virtual async Task RemoveAsync(TEntity entity)
    {
        Entities.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<TEntity?> FirstOrDefault(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = ""
    )
    {

        IQueryable<TEntity> query = this.Entities;
        if (filter != null) query = query.AsNoTracking().Where(filter);
        query = IncludeProperties(includeProperties, query);

        if (orderBy != null) return await orderBy(query.AsNoTracking()).FirstOrDefaultAsync();

        return await query.AsNoTracking().FirstOrDefaultAsync();

    }

    public async Task<IReadOnlyList<TEntity>> Filter(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {

        IQueryable<TEntity> query = this.Entities;
        if (filter != null) query = query.AsNoTracking().Where(filter);
        query = IncludeProperties(includeProperties, query);

        if (orderBy != null) return await orderBy(query.AsNoTracking()).ToListAsync();

        return await query.AsNoTracking().ToListAsync();

    }

    private IQueryable<TEntity> IncludeProperties(string includeProperties, IQueryable<TEntity> query)
    {
        foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

    #endregion
}


