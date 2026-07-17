namespace MyService.Common;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereIfPresent<T>(
        this IEnumerable<T> source, string? value, Func<T, string> selector)
    {
        return string.IsNullOrEmpty(value)
            ? source
            : source.Where(c => selector(c).Contains(value));
    }
}
