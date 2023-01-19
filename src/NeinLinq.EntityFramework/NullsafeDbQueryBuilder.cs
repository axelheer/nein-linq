namespace NeinLinq;

/// <summary>
/// Makes a query null-safe.
/// </summary>
public static class NullsafeDbQueryBuilder
{
    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable ToDbNullsafe(this IQueryable value)
        => value.DbRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable ToDbNullsafe(this IOrderedQueryable value)
        => value.DbRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable<T> ToDbNullsafe<T>(this IQueryable<T> value)
        => value.DbRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable<T> ToDbNullsafe<T>(this IOrderedQueryable<T> value)
        => value.DbRewrite(new NullsafeQueryRewriter());
}
