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

    /// <summary>更新节点基本信息，Status 变化时触发双向联动</summary>
    public async Task<TreeNodeDto?> UpdateAsync(Guid id, UpdateTreeNodeRequest request)
    {
        var node = await _repository.GetByIdAsync(id);
        if (node is null) return null;

        var oldStatus = node.Status;
        node.Name = request.Name;
        node.Description = request.Description;
        node.Status = request.Status;
        node.ModelId = request.ModelId;
        node.UpdatedAt = DateTime.UtcNow;

        if (oldStatus != node.Status)
        {
            // 保存自身，触发变更跟踪
            await _repository.UpdateAsync(node);
            // 父变子：CTE 递归更新所有后代
            await _repository.UpdateDescendantsStatusAsync(id, node.Status);
            // 子变父：逐层向上聚合祖先状态
            await UpdateAncestorStatusesAsync(node);
        }
        else
        {
            await _repository.UpdateAsync(node);
        }

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

    /// <summary>批量更新节点状态，每个节点触发双向联动</summary>
    public async Task<int> BatchUpdateStatusAsync(BatchUpdateStatusRequest request)
    {
        var count = 0;
        foreach (var id in request.Ids.Distinct())
        {
            var node = await _repository.GetByIdAsync(id);
            if (node is null || node.Status == request.Status) continue;

            node.Status = request.Status;
            node.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(node);
            count++;

            // 父变子：CTE 递归更新所有后代
            await _repository.UpdateDescendantsStatusAsync(id, request.Status);
            // 子变父：逐层向上聚合祖先状态
            await UpdateAncestorStatusesAsync(node);
        }
        return count;
    }

    /// <summary>逐层向上聚合祖先节点状态</summary>
    private async Task UpdateAncestorStatusesAsync(TreeNode node)
    {
        var ancestors = await _repository.GetAncestorsAsync(node.Id);
        // ancestors 从父节点到根，按 Level 升序（父→祖父→曾祖父）
        var modified = new List<TreeNode>();

        foreach (var ancestor in ancestors.OrderByDescending(a => a.Level))
        {
            var children = await _repository.GetChildrenAsync(ancestor.Id);
            var newStatus = AggregateChildStatus(children);
            if (ancestor.Status == newStatus) continue;
            ancestor.Status = newStatus;
            ancestor.UpdatedAt = DateTime.UtcNow;
            modified.Add(ancestor);
        }

        if (modified.Count > 0)
            await _repository.UpdateRangeAsync(modified);
    }

    /// <summary>聚合子节点状态：全部完成→完成，有进行中→进行中，否则未完成</summary>
    private static int AggregateChildStatus(IEnumerable<TreeNode> children)
    {
        var list = children.ToList();
        if (list.Count == 0) return (int)Domain.Enums.TreeNodeStatus.未完成;
        if (list.Any(c => c.Status == (int)Domain.Enums.TreeNodeStatus.进行中))
            return (int)Domain.Enums.TreeNodeStatus.进行中;
        if (list.All(c => c.Status == (int)Domain.Enums.TreeNodeStatus.已完成))
            return (int)Domain.Enums.TreeNodeStatus.已完成;
        return (int)Domain.Enums.TreeNodeStatus.未完成;
    }
}
