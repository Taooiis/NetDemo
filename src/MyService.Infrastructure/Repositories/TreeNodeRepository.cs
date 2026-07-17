using Microsoft.EntityFrameworkCore;
using MyService.Domain.Entities;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;

namespace MyService.Infrastructure.Repositories;

/// <summary>树节点仓储实现（继承通用 CRUD，扩展树结构方法）</summary>
public class TreeNodeRepository : Repository<TreeNode>, ITreeNodeRepository
{
    public TreeNodeRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TreeNode>> GetChildrenAsync(Guid parentId)
    {
        return await Context.Set<TreeNode>()
            .Where(n => n.ParentId == parentId)
            .OrderBy(n => n.SortOrder)
            .ToListAsync();
    }

    public async Task<int> GetMaxSortOrderAsync(Guid? parentId)
    {
        var max = await Context.Set<TreeNode>()
            .Where(n => n.ParentId == parentId)
            .MaxAsync(n => (int?)n.SortOrder);
        return max ?? 0;
    }

    public async Task UpdateDescendantsStatusAsync(Guid nodeId, int status)
    {
        var sql = @"
            WITH RECURSIVE descendants AS (
                SELECT Id FROM TreeNodes WHERE Id = {0}
                UNION ALL
                SELECT t.Id FROM TreeNodes t
                INNER JOIN descendants d ON t.ParentId = d.Id
            )
            UPDATE TreeNodes SET Status = {1}, UpdatedAt = NOW()
            WHERE Id IN (SELECT Id FROM descendants)";

        await Context.Database.ExecuteSqlRawAsync(sql, nodeId, status);
    }

    public async Task<List<TreeNode>> GetAncestorsAsync(Guid nodeId)
    {
        var sql = @"
            WITH RECURSIVE ancestors AS (
                SELECT * FROM TreeNodes WHERE Id = (SELECT ParentId FROM TreeNodes WHERE Id = {0})
                UNION ALL
                SELECT t.* FROM TreeNodes t
                INNER JOIN ancestors a ON t.Id = a.ParentId
            )
            SELECT * FROM ancestors";

        return await Context.Set<TreeNode>()
            .FromSqlRaw(sql, nodeId)
            .ToListAsync();
    }
}
