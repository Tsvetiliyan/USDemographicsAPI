using System.Linq.Expressions;

namespace USDemographicsAPI.Core.Interfaces.IRepos;

public interface IRepository<T> : IDisposable where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

    Task<IEnumerable<T>> GetRangeAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

    Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

    void Add(T entity);

    void AddRange(IEnumerable<T> entities);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);

    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    int SaveChanges();

}
