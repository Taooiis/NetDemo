using MyService.Domain.Common;

namespace MyService.Domain.Entities;

/// <summary>建筑构件实体，记录构件在项目中的位置与属性信息</summary>
public class Component : BaseEntity
{
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
}
