using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyService.Domain.Entities;

namespace MyService.Infrastructure.Data.Configurations;

/// <summary>TreeNode 实体映射配置</summary>
public class TreeNodeConfiguration : IEntityTypeConfiguration<TreeNode>
{
    public void Configure(EntityTypeBuilder<TreeNode> builder)
    {
        // 表名
        builder.ToTable("TreeNodes");

        // 主键
        builder.HasKey(n => n.Id);

        // 名称：最长 100，必填
        builder.Property(n => n.Name)
            .HasMaxLength(100)
            .IsRequired();

        // 编码：最长 100，必填，唯一索引
        builder.Property(n => n.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(n => n.Code)
            .IsUnique();

        // 描述：最长 500
        builder.Property(n => n.Description)
            .HasMaxLength(500);

        // 状态：int 直接映射
        builder.Property(n => n.Status)
            .HasDefaultValue(0);

        // 父节点 ID 索引（CTE 递归查询和父子过滤用）
        builder.HasIndex(n => n.ParentId);

        // 自引用外键：ParentId → Id，级联行为设为 Restrict 防止循环删除
        builder.HasOne(n => n.Parent)
            .WithMany(n => n.Children)
            .HasForeignKey(n => n.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
