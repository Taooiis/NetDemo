using System.ComponentModel.DataAnnotations;

namespace MyService.Application.DTOs;

/// <summary>创建树节点请求</summary>
public class CreateTreeNodeRequest
{
    /// <summary>节点名称（最长 100 字）</summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>节点描述</summary>
    public string? Description { get; set; }

    /// <summary>状态</summary>
    public bool Status { get; set; } = true;

    /// <summary>关联业务表 ID</summary>
    public Guid? ModelId { get; set; }

    /// <summary>父节点 ID（null 表示根节点）</summary>
    public Guid? ParentId { get; set; }

    /// <summary>排序号（可选，不传则排到最后）</summary>
    public int? SortOrder { get; set; }
}

/// <summary>更新树节点请求</summary>
public class UpdateTreeNodeRequest
{
    /// <summary>节点名称（最长 100 字）</summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>节点描述</summary>
    public string? Description { get; set; }

    /// <summary>状态</summary>
    public bool Status { get; set; } = true;

    /// <summary>关联业务表 ID</summary>
    public Guid? ModelId { get; set; }
}

/// <summary>移动树节点请求</summary>
public class MoveTreeNodeRequest
{
    /// <summary>目标父节点 ID（null 表示移至根层级）</summary>
    public Guid? ParentId { get; set; }

    /// <summary>目标排序号</summary>
    public int SortOrder { get; set; }
}

/// <summary>树节点输出 DTO（含递归 Children）</summary>
public class TreeNodeDto
{
    /// <summary>节点 ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>节点编码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>节点描述</summary>
    public string? Description { get; set; }

    /// <summary>状态</summary>
    public bool Status { get; set; }

    /// <summary>关联业务表 ID</summary>
    public Guid? ModelId { get; set; }

    /// <summary>父节点 ID</summary>
    public Guid? ParentId { get; set; }

    /// <summary>层级</summary>
    public int Level { get; set; }

    /// <summary>排序号</summary>
    public int SortOrder { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>最后更新时间</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>子节点列表（递归嵌套）</summary>
    public List<TreeNodeDto> Children { get; set; } = new();
}
