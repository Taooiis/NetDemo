using MyService.Application.DTOs;

namespace MyService.Application.Interfaces;

/// <summary>树节点服务接口</summary>
public interface ITreeNodeService
{
    /// <summary>获取整棵树（内存组装层级结构）</summary>
    Task<IEnumerable<TreeNodeDto>> GetTreeAsync();

    /// <summary>根据 ID 获取单个节点</summary>
    Task<TreeNodeDto?> GetByIdAsync(Guid id);

    /// <summary>获取指定父节点的直接子节点</summary>
    Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(Guid parentId);

    /// <summary>创建节点（自动计算 Level 和 SortOrder）</summary>
    Task<TreeNodeDto?> CreateAsync(CreateTreeNodeRequest request);

    /// <summary>更新节点基本信息</summary>
    Task<TreeNodeDto?> UpdateAsync(Guid id, UpdateTreeNodeRequest request);

    /// <summary>移动节点（更换父节点或重排序）</summary>
    Task<TreeNodeDto?> MoveAsync(Guid id, MoveTreeNodeRequest request);

    /// <summary>批量更新节点状态（含父子联动）</summary>
    Task<int> BatchUpdateStatusAsync(BatchUpdateStatusRequest request);

    /// <summary>删除节点及所有子节点</summary>
    Task<bool> DeleteAsync(Guid id);
}
