namespace Domain.Abstractions;

public interface IGenericRepository<TSource> where TSource : class
{
    Task<TSource> SaveAsync(TSource entity);
    Task<TSource> UpdateAsync(TSource entity);
    Task<TSource?> GetByIdAsync(long id);
    Task<IReadOnlyList<TSource>> GetAllAsync();
}
