using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyService.Domain.Common;
using MyService.Domain.Interfaces;
using MyService.Infrastructure.Data;

namespace MyService.Infrastructure.Repositories;

/// <summary>通用仓储实现</summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;

    public Repository(AppDbContext context)
    {
        Context = context;
    }

    public virtual IQueryable<T> Query => Context.Set<T>();

    public async Task<List<T>> ToListAsync(IQueryable<T> query) => await query.ToListAsync();
    public async Task<T?> FirstOrDefaultAsync(IQueryable<T> query) => await query.FirstOrDefaultAsync();
    public async Task<bool> AnyAsync(IQueryable<T> query) => await query.AnyAsync();
    public async Task<int> CountAsync(IQueryable<T> query) => await query.CountAsync();

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, int batchSize = 1000)
    {
        var list = entities.ToList();
        for (var i = 0; i < list.Count; i += batchSize)
        {
            var batch = list.Skip(i).Take(batchSize);
            await Context.Set<T>().AddRangeAsync(batch);
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        if (list.Count == 0) return;
        Context.Set<T>().RemoveRange(list);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return;

        await DeleteRangeAsync(idList, e => e.Id);
    }

    public virtual async Task DeleteRangeAsync<TProperty>(
        IEnumerable<TProperty> values, Expression<Func<T, TProperty>> propertySelector)
    {
        var valueList = values.ToList();
        if (valueList.Count == 0) return;

        var param = Expression.Parameter(typeof(T), "e");
        var prop = Expression.Property(param, ((MemberExpression)propertySelector.Body).Member.Name);
        Expression? body = null;
        foreach (var value in valueList)
        {
            var eq = Expression.Equal(prop, Expression.Constant(value, typeof(TProperty)));
            body = body is null ? eq : Expression.OrElse(body, eq);
        }

        var predicate = Expression.Lambda<Func<T, bool>>(body!, param);
        var entities = await Context.Set<T>().Where(predicate).ToListAsync();
        Context.Set<T>().RemoveRange(entities);
        await Context.SaveChangesAsync();
    }
}
