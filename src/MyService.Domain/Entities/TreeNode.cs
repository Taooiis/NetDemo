using MyService.Domain.Common;

namespace MyService.Domain.Entities;

/// <summary>树节点实体，邻接表模型，支持六层树形结构</summary>
public class TreeNode : BaseEntity
{
    /// <summary>节点名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>节点编码（唯一）</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>节点描述</summary>
    public string? Description { get; set; }

    /// <summary>状态：true=启用，false=禁用</summary>
    public bool Status { get; set; } = true;

    /// <summary>关联业务表 ID</summary>
    public Guid? ModelId { get; set; }

    /// <summary>父节点 ID（null 表示根节点，Level=0）</summary>
    public Guid? ParentId { get; set; }

    /// <summary>层级（0~5）</summary>
    public int Level { get; set; }

    /// <summary>同级排序序号（升序）</summary>
    public int SortOrder { get; set; }

    /// <summary>父节点导航属性</summary>
    public TreeNode? Parent { get; set; }

    /// <summary>子节点集合</summary>
    public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();
}
