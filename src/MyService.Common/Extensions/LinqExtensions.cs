using System.Linq.Expressions;

namespace MyService.Common;

public static class LinqExtensions
{
    public static IEnumerable<T> WhereIfPresent<T>(
        this IEnumerable<T> source, string? value, Func<T, string> selector)
    {
        return string.IsNullOrEmpty(value)
            ? source
            : source.Where(c => selector(c).Contains(value));
    }

    public static IQueryable<T> WhereIfPresent<T>(
        this IQueryable<T> source, string? value, Expression<Func<T, string>> selector)
    {
        if (string.IsNullOrEmpty(value)) return source;

        var param = ((LambdaExpression)selector).Parameters[0];
        var contains = typeof(string).GetMethod("Contains", [typeof(string)])!;
        var body = Expression.Call(selector.Body, contains, Expression.Constant(value));
        var predicate = Expression.Lambda<Func<T, bool>>(body, param);
        return source.Where(predicate);
    }
}
