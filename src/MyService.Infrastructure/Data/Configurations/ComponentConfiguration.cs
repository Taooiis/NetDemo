using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyService.Domain.Entities;

namespace MyService.Infrastructure.Data.Configurations;

/// <summary>构件实体 EF Core 配置</summary>
public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.ToTable("Components");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ComponentId).IsRequired();
        builder.Property(e => e.ProjectCode).HasMaxLength(100);
        builder.Property(e => e.BuildingCode).HasMaxLength(100);
        builder.Property(e => e.BuildingName).HasMaxLength(200);
        builder.Property(e => e.FloorCode).HasMaxLength(100);
        builder.Property(e => e.FloorName).HasMaxLength(200);
        builder.Property(e => e.ComponentCode).HasMaxLength(200);
        builder.Property(e => e.ComponentName).HasMaxLength(200);
        builder.Property(e => e.ComponentType).HasMaxLength(100);
        builder.Property(e => e.ComponentTypeName).HasMaxLength(200);
        builder.Property(e => e.PartCode).HasMaxLength(100);
        builder.Property(e => e.ModelPartKey).HasMaxLength(300);
        builder.Property(e => e.Name).HasMaxLength(200);
        builder.Property(e => e.SourceCollectionPath).HasMaxLength(500);
        builder.Property(e => e.SourceObjectName).HasMaxLength(200);
        builder.Property(e => e.BindingStatus).HasMaxLength(50);

        builder.HasIndex(e => e.ComponentId);
        builder.HasIndex(e => e.ComponentCode);
    }
}
