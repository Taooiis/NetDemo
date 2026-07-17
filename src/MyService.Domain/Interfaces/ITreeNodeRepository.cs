using MyService.Domain.Entities;

namespace MyService.Domain.Interfaces;

/// <summary>树节点仓储接口（继承通用 CRUD，扩展树结构方法）</summary>
public interface ITreeNodeRepository : IRepository<TreeNode>
{
    /// <summary>获取指定父节点的直接子节点列表</summary>
    Task<IEnumerable<TreeNode>> GetChildrenAsync(Guid parentId);

    /// <summary>获取指定父节点下同级最大排序号</summary>
    Task<int> GetMaxSortOrderAsync(Guid? parentId);
}
