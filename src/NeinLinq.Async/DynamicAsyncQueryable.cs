namespace NeinLinq;

/// <summary>
/// Helps building dynamic async queries.
/// </summary>
public static class DynamicAsyncQueryable
{
    /// <summary>
    /// Create a dynamic where clause for a given property selector, comparison method and reference value.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The filtered query.</returns>
    public static IAsyncQueryable<T> Where<T>(this IAsyncQueryable<T> query,
                                              string selector,
                                              DynamicCompare comparer,
                                              string? value,
                                              IFormatProvider? provider = null)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
            throw new ArgumentOutOfRangeException(nameof(comparer));

        var target = Expression.Parameter(typeof(T));

        var comparison = DynamicExpression.CreateComparison(target, selector, comparer, value, provider);

        return query.Provider.CreateQuery<T>(
            CreateAsyncWhereClause(target, query.Expression, comparison));
    }

    /// <summary>
    /// Create a dynamic where clause for a given property selector, comparison method and reference value.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The filtered query.</returns>
    public static IAsyncQueryable<T> Where<T>(this IAsyncQueryable<T> query,
                                              string selector,
                                              string comparer,
                                              string? value,
                                              IFormatProvider? provider = null)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (string.IsNullOrEmpty(comparer))
            throw new ArgumentNullException(nameof(comparer));

        var target = Expression.Parameter(typeof(T));

        var comparison = DynamicExpression.CreateComparison(target, selector, comparer, value, provider);

        return query.Provider.CreateQuery<T>(
            CreateAsyncWhereClause(target, query.Expression, comparison));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedAsyncQueryable<T> OrderBy<T>(this IAsyncQueryable<T> query,
                                                       string selector,
                                                       bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(typeof(T));

        return (IOrderedAsyncQueryable<T>)query.Provider.CreateQuery<T>(
            CreateAsyncOrderClause(target, query.Expression, selector, true, descending));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedAsyncQueryable<T> ThenBy<T>(this IOrderedAsyncQueryable<T> query,
                                                      string selector,
                                                      bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(typeof(T));

        return (IOrderedAsyncQueryable<T>)query.Provider.CreateQuery<T>(
            CreateAsyncOrderClause(target, query.Expression, selector, false, descending));
    }

    private static MethodCallExpression CreateAsyncOrderClause(ParameterExpression target,
                                                               Expression expression,
                                                               string selector,
                                                               bool initial,
                                                               bool descending)
    {
        var keySelector = Expression.Lambda(DynamicExpression.CreateMemberAccess(target, selector), target);

        var method = initial ? (descending ? nameof(AsyncQueryable.OrderByDescending)
                                           : nameof(AsyncQueryable.OrderBy))
                             : (descending ? nameof(AsyncQueryable.ThenByDescending)
                                           : nameof(AsyncQueryable.ThenBy));

        return Expression.Call(typeof(AsyncQueryable), method, [target.Type, keySelector.ReturnType],
            expression, Expression.Quote(keySelector));
    }

    private static MethodCallExpression CreateAsyncWhereClause(ParameterExpression target,
                                                               Expression expression,
                                                               Expression comparison)
    {
        var predicate = Expression.Lambda(comparison, target);

        return Expression.Call(typeof(AsyncQueryable), nameof(AsyncQueryable.Where), [target.Type],
            expression, Expression.Quote(predicate));
    }
}
