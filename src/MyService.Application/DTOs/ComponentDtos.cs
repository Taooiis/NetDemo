using System.ComponentModel.DataAnnotations;

namespace MyService.Application.DTOs;

/// <summary>构件输出 DTO</summary>
public class ComponentDto
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string BuildingCode { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string FloorCode { get; set; } = string.Empty;
    public string FloorName { get; set; } = string.Empty;
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentTypeName { get; set; } = string.Empty;
    public string PartCode { get; set; } = string.Empty;
    public string ModelPartKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SourceCollectionPath { get; set; } = string.Empty;
    public string SourceObjectName { get; set; } = string.Empty;
    public string BindingStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>创建构件请求</summary>
public class CreateComponentRequest
{
    [Required]
    public Guid ComponentId { get; set; }
    [Required, StringLength(100)]
    public string ProjectCode { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string BuildingCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string BuildingName { get; set; } = string.Empty;
    [StringLength(100)]
    public string FloorCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string FloorName { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string ComponentCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string ComponentName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string ComponentType { get; set; } = string.Empty;
    [StringLength(200)]
    public string ComponentTypeName { get; set; } = string.Empty;
    [StringLength(100)]
    public string PartCode { get; set; } = string.Empty;
    [StringLength(300)]
    public string ModelPartKey { get; set; } = string.Empty;
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(500)]
    public string SourceCollectionPath { get; set; } = string.Empty;
    [StringLength(200)]
    public string SourceObjectName { get; set; } = string.Empty;
    [StringLength(50)]
    public string BindingStatus { get; set; } = string.Empty;
}

/// <summary>构件搜索请求（传入非空的字段即作为过滤条件，多个条件取交集，模糊匹配）</summary>
public class ComponentSearchRequest
{
    public string? ProjectCode { get; set; }
    public string? BuildingCode { get; set; }
    public string? BuildingName { get; set; }
    public string? FloorCode { get; set; }
    public string? FloorName { get; set; }
    public string? ComponentCode { get; set; }
    public string? ComponentName { get; set; }
    public string? ComponentType { get; set; }
    public string? ComponentTypeName { get; set; }
    public string? PartCode { get; set; }
    public string? ModelPartKey { get; set; }
    public string? Name { get; set; }
    public string? SourceCollectionPath { get; set; }
    public string? SourceObjectName { get; set; }
    public string? BindingStatus { get; set; }
}

/// <summary>更新构件请求</summary>
public class UpdateComponentRequest
{
    [Required]
    public Guid ComponentId { get; set; }
    [Required, StringLength(100)]
    public string ProjectCode { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string BuildingCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string BuildingName { get; set; } = string.Empty;
    [StringLength(100)]
    public string FloorCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string FloorName { get; set; } = string.Empty;
    [Required, StringLength(200)]
    public string ComponentCode { get; set; } = string.Empty;
    [StringLength(200)]
    public string ComponentName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string ComponentType { get; set; } = string.Empty;
    [StringLength(200)]
    public string ComponentTypeName { get; set; } = string.Empty;
    [StringLength(100)]
    public string PartCode { get; set; } = string.Empty;
    [StringLength(300)]
    public string ModelPartKey { get; set; } = string.Empty;
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    [StringLength(500)]
    public string SourceCollectionPath { get; set; } = string.Empty;
    [StringLength(200)]
    public string SourceObjectName { get; set; } = string.Empty;
    [StringLength(50)]
    public string BindingStatus { get; set; } = string.Empty;
}
