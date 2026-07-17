using MyService.Domain.Entities;

namespace MyService.Domain.Interfaces;

/// <summary>树节点仓储接口（继承通用 CRUD，扩展树结构方法）</summary>
public interface ITreeNodeRepository : IRepository<TreeNode>
{
    /// <summary>获取指定父节点的直接子节点列表</summary>
    Task<IEnumerable<TreeNode>> GetChildrenAsync(Guid parentId);

    /// <summary>获取指定父节点下同级最大排序号</summary>
    Task<int> GetMaxSortOrderAsync(Guid? parentId);

    /// <summary>递归 CTE 更新指定节点及其所有后代的状态</summary>
    Task UpdateDescendantsStatusAsync(Guid nodeId, int status);

    /// <summary>递归 CTE 获取从父节点到根的所有祖先节点</summary>
    Task<List<TreeNode>> GetAncestorsAsync(Guid nodeId);
}
