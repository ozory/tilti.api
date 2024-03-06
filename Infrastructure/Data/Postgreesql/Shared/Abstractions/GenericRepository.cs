using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

using DomainUser = Domain.Features.Users.Entities.User;
using InfrastructureUser = Infrastructure.Data.Postgreesql.Features.Users.Entities.User;

namespace Infrastructure.Data.Postgreesql.Shared;

public class GenericRepository<TSource, TDestination>
    where TSource : class
    where TDestination : class
{
    protected readonly TILTContext _context;

    internal DbSet<TDestination> dbSet;

    protected GenericRepository(TILTContext context)
    {
        _context = context;
        this.dbSet = _context.Set<TDestination>();
    }

    public async Task<IReadOnlyList<TSource>> GetAllAsync()
    {
        var resources = await this.dbSet.AsNoTracking().ToListAsync();
        var userList = resources.Select(u => u as TSource).ToList();
        return userList!;
    }

    public async Task<TSource?> GetByIdAsync(long id)
    {
        var user = await this.dbSet.FindAsync(id);
        return user as TSource;
    }

    public async Task<TSource> SaveAsync(TSource entity)
    {
        await this.dbSet.AddAsync((entity as TDestination)!);
        return entity;
    }

    public async Task<TSource> UpdateAsync(TSource entity)
    {
        var destination = entity as TDestination;
        dbSet.Attach(destination!);
        _context.Entry(destination!).State = EntityState.Modified;
        await Task.CompletedTask;
        return (destination as TSource)!;
    }
}


