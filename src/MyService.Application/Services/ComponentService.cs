using MyService.Application.DTOs;
using MyService.Application.Interfaces;
using MyService.Common;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;

namespace MyService.Application.Services;

/// <summary>构件服务实现</summary>
public class ComponentService : IComponentService
{
    private readonly IRepository<Component> _repository;

    public ComponentService(IRepository<Component> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ComponentDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(MapToDto);
    }

    public async Task<IEnumerable<ComponentDto>> SearchAsync(ComponentSearchRequest request)
    {
        if (request is null) return await GetAllAsync();

        var query = _repository.Query;
        query = query.WhereIfPresent(request.ProjectCode, c => c.ProjectCode)
                     .WhereIfPresent(request.BuildingCode, c => c.BuildingCode)
                     .WhereIfPresent(request.BuildingName, c => c.BuildingName)
                     .WhereIfPresent(request.FloorCode, c => c.FloorCode)
                     .WhereIfPresent(request.FloorName, c => c.FloorName)
                     .WhereIfPresent(request.ComponentCode, c => c.ComponentCode)
                     .WhereIfPresent(request.ComponentName, c => c.ComponentName)
                     .WhereIfPresent(request.ComponentType, c => c.ComponentType)
                     .WhereIfPresent(request.ComponentTypeName, c => c.ComponentTypeName)
                     .WhereIfPresent(request.PartCode, c => c.PartCode)
                     .WhereIfPresent(request.ModelPartKey, c => c.ModelPartKey)
                     .WhereIfPresent(request.Name, c => c.Name)
                     .WhereIfPresent(request.SourceCollectionPath, c => c.SourceCollectionPath)
                     .WhereIfPresent(request.SourceObjectName, c => c.SourceObjectName)
                     .WhereIfPresent(request.BindingStatus, c => c.BindingStatus);
        return (await _repository.ToListAsync(query)).Select(MapToDto);
    }

    public async Task<ComponentDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<ComponentDto?> CreateAsync(CreateComponentRequest request)
    {
        await _repository.DeleteRangeAsync(
            await _repository.FindAsync(c => c.ComponentId == request.ComponentId));

        var entity = MapToEntity(request);
        await _repository.AddAsync(entity);
        return MapToDto(entity);
    }

    public async Task<int> CreateRangeAsync(IEnumerable<CreateComponentRequest> requests)
    {
        var requestList = requests.ToList();
        var componentIds = requestList.Select(r => r.ComponentId).ToList();

        await _repository.DeleteRangeAsync(
            await _repository.FindAsync(c => componentIds.Contains(c.ComponentId)));

        var entities = requestList.Select(MapToEntity).ToList();
        await _repository.AddRangeAsync(entities);
        return entities.Count;
    }

    private static Component MapToEntity(CreateComponentRequest request) => new()
    {
        ComponentId = request.ComponentId,
        ProjectCode = request.ProjectCode,
        BuildingCode = request.BuildingCode,
        BuildingName = request.BuildingName,
        FloorCode = request.FloorCode,
        FloorName = request.FloorName,
        ComponentCode = request.ComponentCode,
        ComponentName = request.ComponentName,
        ComponentType = request.ComponentType,
        ComponentTypeName = request.ComponentTypeName,
        PartCode = request.PartCode,
        ModelPartKey = request.ModelPartKey,
        Name = request.Name,
        SourceCollectionPath = request.SourceCollectionPath,
        SourceObjectName = request.SourceObjectName,
        BindingStatus = request.BindingStatus,
    };

    public async Task<ComponentDto?> UpdateAsync(Guid id, UpdateComponentRequest request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return null;

        entity.ComponentId = request.ComponentId;
        entity.ProjectCode = request.ProjectCode;
        entity.BuildingCode = request.BuildingCode;
        entity.BuildingName = request.BuildingName;
        entity.FloorCode = request.FloorCode;
        entity.FloorName = request.FloorName;
        entity.ComponentCode = request.ComponentCode;
        entity.ComponentName = request.ComponentName;
        entity.ComponentType = request.ComponentType;
        entity.ComponentTypeName = request.ComponentTypeName;
        entity.PartCode = request.PartCode;
        entity.ModelPartKey = request.ModelPartKey;
        entity.Name = request.Name;
        entity.SourceCollectionPath = request.SourceCollectionPath;
        entity.SourceObjectName = request.SourceObjectName;
        entity.BindingStatus = request.BindingStatus;

        await _repository.UpdateAsync(entity);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    private static ComponentDto MapToDto(Component entity) => new()
    {
        Id = entity.Id,
        ComponentId = entity.ComponentId,
        ProjectCode = entity.ProjectCode,
        BuildingCode = entity.BuildingCode,
        BuildingName = entity.BuildingName,
        FloorCode = entity.FloorCode,
        FloorName = entity.FloorName,
        ComponentCode = entity.ComponentCode,
        ComponentName = entity.ComponentName,
        ComponentType = entity.ComponentType,
        ComponentTypeName = entity.ComponentTypeName,
        PartCode = entity.PartCode,
        ModelPartKey = entity.ModelPartKey,
        Name = entity.Name,
        SourceCollectionPath = entity.SourceCollectionPath,
        SourceObjectName = entity.SourceObjectName,
        BindingStatus = entity.BindingStatus,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
    };
}
