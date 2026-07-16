using System.Linq.Expressions;
using System.Reflection;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;
using MyService.Domain.Common;
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

    private static readonly PropertyInfo[] _searchableProps = typeof(Component)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.PropertyType == typeof(string))
        .ToArray();

    public async Task<IEnumerable<ComponentDto>> SearchAsync(ComponentSearchRequest request)
    {
        if (request is null) return await GetAllAsync();

        var param = Expression.Parameter(typeof(Component), "c");
        Expression? body = null;
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var requestProps = typeof(ComponentSearchRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var rp in requestProps)
        {
            var value = rp.GetValue(request) as string;
            if (string.IsNullOrEmpty(value)) continue;

            var prop = _searchableProps.FirstOrDefault(p =>
                p.Name.Equals(rp.Name, StringComparison.OrdinalIgnoreCase));
            if (prop is null) continue;

            var member = Expression.MakeMemberAccess(param, prop);
            var condition = Expression.Call(member, containsMethod, Expression.Constant(value));
            body = body is null ? condition : Expression.AndAlso(body, condition);
        }

        if (body is null) return await GetAllAsync();

        var lambda = Expression.Lambda<Func<Component, bool>>(body, param);
        var list = await _repository.FindAsync(lambda);
        return list.Select(MapToDto);
    }

    public async Task<ComponentDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<ComponentDto?> CreateAsync(CreateComponentRequest request)
    {
        var entity = MapToEntity(request);
        await _repository.AddAsync(entity);
        return MapToDto(entity);
    }

    public async Task<int> CreateRangeAsync(IEnumerable<CreateComponentRequest> requests)
    {
        var entities = requests.Select(MapToEntity).ToList();
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
