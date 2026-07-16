using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyService.Domain.Common;

namespace MyService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 反射自动注册所有继承 BaseEntity 的实体，新增实体无需手动添加 DbSet
        var entityTypes = typeof(BaseEntity).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.IsSubclassOf(typeof(BaseEntity)));

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}