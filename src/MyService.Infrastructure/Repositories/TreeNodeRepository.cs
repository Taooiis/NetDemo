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
}
