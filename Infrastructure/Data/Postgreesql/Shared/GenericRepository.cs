using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Shared;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly TILTContext _context;

    protected GenericRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> SaveAsync(T entity)
    {
        await _context.AddAsync(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Update(entity);
        await Task.CompletedTask;
        return entity;
    }
}


