using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using Domain.Abstractions;
using Domain.Shared.Abstractions;
using Infrastructure.Data.Postgreesql.Shared.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<TDestination>
    where TDestination : InfrastructureEntity
{
    protected readonly TILTContext _context;

    internal DbSet<TDestination> dbSet;

    protected GenericRepository(TILTContext context)
    {
        _context = context;
        this.dbSet = _context.Set<TDestination>();
    }

    public virtual async Task<IReadOnlyList<TDestination>> GetAllAsyncSourced()
    {
        return await this.dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IReadOnlyList<TDestination>> GetAllAsync()
    {
        return await this.dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<TDestination?> GetByIdAsync(long id)
    {
        var entity = await this.dbSet.FindAsync(id);
        return entity;
    }

    public virtual async Task<TDestination> SaveAsync(TDestination entity)
    {
        await this.dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task<TDestination> UpdateAsync(TDestination entity)
    {
        dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
        return entity;
    }

    public async Task<TDestination?> FirstOrDefault(Expression<Func<TDestination, bool>> predicate)
    {
        return await this.dbSet.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TDestination>> Filter(Expression<Func<TDestination, bool>> predicate)
    {
        return await this.dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }
}


