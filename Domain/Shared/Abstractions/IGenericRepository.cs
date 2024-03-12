using System.Linq.Expressions;

namespace Domain.Abstractions;

public interface IGenericRepository<TSource> where TSource : class
{
    Task<TSource> SaveAsync(TSource entity);
    Task<TSource> UpdateAsync(TSource entity);
    Task<TSource?> GetByIdAsync(long id);
    Task<IReadOnlyList<TSource>> GetAllAsync();
    Task<TSource?> FirstOrDefault(Expression<Func<TSource, bool>> predicate);
    Task<IReadOnlyList<TSource>> Filter(Expression<Func<TSource, bool>> predicate);
}
