namespace Domain.Abstractions;

public interface IGenericRepository<T> where T : class
{
    Task<T> SaveAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T?> GetByIdAsync(long id);
    Task<IReadOnlyList<T>> GetAllAsync();
}
