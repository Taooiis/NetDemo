using MyService.Application.DTOs;
using MyService.Application.Interfaces;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;

namespace MyService.Application.Services;

/// <summary>树节点服务实现，包含核心业务逻辑</summary>
public class TreeNodeService : ITreeNodeService
{
    private readonly ITreeNodeRepository _repository;

    public TreeNodeService(ITreeNodeRepository repository)
    {
        _repository = repository;
    }

    /// <summary>获取整棵树，一次性查出所有节点并在内存中递归组装</summary>
    public async Task<IEnumerable<TreeNodeDto>> GetTreeAsync()
    {
        var allNodes = await _repository.GetAllAsync();
        return BuildTree(allNodes, null);
    }

    /// <summary>根据 ID 获取单个节点</summary>
    public async Task<TreeNodeDto?> GetByIdAsync(Guid id)
    {
        var node = await _repository.GetByIdAsync(id);
        return node is null ? null : MapToDto(node);
    }

    /// <summary>获取指定父节点的直接子节点列表</summary>
    public async Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(Guid parentId)
    {
        var children = await _repository.GetChildrenAsync(parentId);
        return children.Select(MapToDto);
    }

    /// <summary>创建节点，自动计算 Level 和 SortOrder</summary>
    public async Task<TreeNodeDto?> CreateAsync(CreateTreeNodeRequest request)
    {
        var sortOrder = request.SortOrder ?? (await _repository.GetMaxSortOrderAsync(request.ParentId) + 1);

        TreeNode node;
        if (request.ParentId is { } parentId)
        {
            var parent = await _repository.GetByIdAsync(parentId);
            if (parent is null) return null;
            if (parent.Level >= 5) return null;

            node = new TreeNode
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = $"{parent.Code}-{sortOrder:d3}",
                Description = request.Description,
                Status = request.Status,
                ModelId = request.ModelId,
                ParentId = parentId,
                Level = parent.Level + 1,
                SortOrder = sortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        else
        {
            node = new TreeNode
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = $"{sortOrder:d3}",
                Description = request.Description,
                Status = request.Status,
                ModelId = request.ModelId,
                ParentId = null,
                Level = 0,
                SortOrder = sortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        await _repository.AddAsync(node);
        return MapToDto(node);
    }

    /// <summary>更新节点基本信息（不改变树结构）</summary>
    public async Task<TreeNodeDto?> UpdateAsync(Guid id, UpdateTreeNodeRequest request)
    {
        var node = await _repository.GetByIdAsync(id);
        if (node is null) return null;

        node.Name = request.Name;
        node.Description = request.Description;
        node.Status = request.Status;
        node.ModelId = request.ModelId;
        node.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(node);
        return MapToDto(node);
    }

    /// <summary>移动节点，更换父节点并自动重算 Level</summary>
    public async Task<TreeNodeDto?> MoveAsync(Guid id, MoveTreeNodeRequest request)
    {
        var node = await _repository.GetByIdAsync(id);
        if (node is null) return null;

        // 重算层级
        if (request.ParentId.HasValue)
        {
            var parent = await _repository.GetByIdAsync(request.ParentId.Value);
            if (parent is null) return null;
            if (parent.Level >= 5) return null;

            node.Level = parent.Level + 1;
        }
        else
        {
            node.Level = 0; // 移至根层级
        }

        node.ParentId = request.ParentId;
        node.SortOrder = request.SortOrder;
        node.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(node);
        return MapToDto(node);
    }

    /// <summary>删除节点及其所有后代节点</summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var node = await _repository.GetByIdAsync(id);
        if (node is null) return false;

        var allNodes = await _repository.GetAllAsync();
        var descendants = GetDescendants(allNodes, id).ToList();
        descendants.Add(node);
        await _repository.DeleteRangeAsync(descendants);
        return true;
    }

    /// <summary>递归构建树形结构</summary>
    /// <param name="allNodes">所有节点</param>
    /// <param name="parentId">当前父节点 ID</param>
    private static List<TreeNodeDto> BuildTree(IEnumerable<TreeNode> allNodes, Guid? parentId)
    {
        return allNodes
            .Where(n => n.ParentId == parentId)
            .OrderBy(n => n.SortOrder)
            .Select(n =>
            {
                var dto = MapToDto(n);
                dto.Children = BuildTree(allNodes, n.Id);
                return dto;
            })
            .ToList();
    }

    /// <summary>递归获取所有后代节点</summary>
    /// <param name="allNodes">所有节点</param>
    /// <param name="parentId">父节点 ID</param>
    private static List<TreeNode> GetDescendants(IEnumerable<TreeNode> allNodes, Guid parentId)
    {
        var result = new List<TreeNode>();
        var children = allNodes.Where(n => n.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            result.Add(child);
            result.AddRange(GetDescendants(allNodes, child.Id));
        }
        return result;
    }

    /// <summary>TreeNode 实体转 DTO</summary>
    private static TreeNodeDto MapToDto(TreeNode node) => new()
    {
        Id = node.Id,
        Name = node.Name,
        Code = node.Code,
        Description = node.Description,
        Status = node.Status,
        ModelId = node.ModelId,
        ParentId = node.ParentId,
        Level = node.Level,
        SortOrder = node.SortOrder,
        CreatedAt = node.CreatedAt,
        UpdatedAt = node.UpdatedAt
    };
}
