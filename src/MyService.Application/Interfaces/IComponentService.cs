using MyService.Application.DTOs;

namespace MyService.Application.Interfaces;

/// <summary>构件服务接口</summary>
public interface IComponentService
{
    Task<IEnumerable<ComponentDto>> GetAllAsync();
    Task<ComponentDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ComponentDto>> SearchAsync(ComponentSearchRequest request);
    Task<ComponentDto?> CreateAsync(CreateComponentRequest request);
    Task<int> CreateRangeAsync(IEnumerable<CreateComponentRequest> requests);
    Task<ComponentDto?> UpdateAsync(Guid id, UpdateComponentRequest request);
    Task<bool> DeleteAsync(Guid id);
}
