namespace NeinLinq;

/// <summary>
/// Makes a query null-safe.
/// </summary>
public static class NullsafeAsyncQueryBuilder
{
    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable ToAsyncNullsafe(this IAsyncQueryable value)
        => value.AsyncRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable ToAsyncNullsafe(this IOrderedAsyncQueryable value)
        => value.AsyncRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable<T> ToAsyncNullsafe<T>(this IAsyncQueryable<T> value)
        => value.AsyncRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable<T> ToAsyncNullsafe<T>(this IOrderedAsyncQueryable<T> value)
        => value.AsyncRewrite(new NullsafeQueryRewriter());
}
