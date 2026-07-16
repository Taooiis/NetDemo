using MyService.Domain.Common;

namespace MyService.Domain.Interfaces;

/// <summary>通用仓储接口，提供基础 CRUD</summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
