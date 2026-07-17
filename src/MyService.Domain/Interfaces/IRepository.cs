using System.Linq.Expressions;
using MyService.Domain.Common;

namespace MyService.Domain.Interfaces;

/// <summary>通用仓储接口，提供基础 CRUD</summary>
public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Query { get; }
    Task<List<T>> ToListAsync(IQueryable<T> query);
    Task<T?> FirstOrDefaultAsync(IQueryable<T> query);
    Task<bool> AnyAsync(IQueryable<T> query);
    Task<int> CountAsync(IQueryable<T> query);
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities, int batchSize = 1000);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task DeleteRangeAsync(IEnumerable<Guid> ids);
    Task DeleteRangeAsync<TProperty>(IEnumerable<TProperty> values, Expression<Func<T, TProperty>> propertySelector);
}
