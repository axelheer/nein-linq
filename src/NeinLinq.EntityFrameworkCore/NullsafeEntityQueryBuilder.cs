namespace NeinLinq;

/// <summary>
/// Makes a query null-safe.
/// </summary>
public static class NullsafeEntityQueryBuilder
{
    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable ToEntityNullsafe(this IQueryable value)
        => value.EntityRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable ToEntityNullsafe(this IOrderedQueryable value)
        => value.EntityRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable<T> ToEntityNullsafe<T>(this IQueryable<T> value)
        => value.EntityRewrite(new NullsafeQueryRewriter());

    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable<T> ToEntityNullsafe<T>(this IOrderedQueryable<T> value)
        => value.EntityRewrite(new NullsafeQueryRewriter());
}
