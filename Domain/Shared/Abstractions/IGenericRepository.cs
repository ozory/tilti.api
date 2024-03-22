using System.Linq.Expressions;

namespace Domain.Abstractions;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> SaveAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task RemoveAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(long id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");
    Task<IReadOnlyList<TEntity>> Filter(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");
}
