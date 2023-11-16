namespace NeinLinq;

/// <summary>
/// Helps building dynamic queries.
/// </summary>
public static class DynamicQueryable
{
    /// <summary>
    /// Create a dynamic where clause for a given property selector, comparison method and reference value.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable Where(this IQueryable query,
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

        var target = Expression.Parameter(query.ElementType);

        var comparison = DynamicExpression.CreateComparison(target, selector, comparer, value, provider);

        return query.Provider.CreateQuery(
            CreateWhereClause(target, query.Expression, comparison));
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
    public static IQueryable<T> Where<T>(this IQueryable<T> query,
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
            CreateWhereClause(target, query.Expression, comparison));
    }

    /// <summary>
    /// Create a dynamic where clause for a given property selector, comparison method and reference value.
    /// </summary>
    /// <param name="query">The query to filter.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable Where(this IQueryable query,
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

        var target = Expression.Parameter(query.ElementType);

        var comparison = DynamicExpression.CreateComparison(target, selector, comparer, value, provider);

        return query.Provider.CreateQuery(
            CreateWhereClause(target, query.Expression, comparison));
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
    public static IQueryable<T> Where<T>(this IQueryable<T> query,
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
            CreateWhereClause(target, query.Expression, comparison));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedQueryable OrderBy(this IQueryable query,
                                            string selector,
                                            bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(query.ElementType);

        return (IOrderedQueryable)query.Provider.CreateQuery(
            CreateOrderClause(target, query.Expression, selector, true, descending));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query,
                                                  string selector,
                                                  bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(typeof(T));

        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(
            CreateOrderClause(target, query.Expression, selector, true, descending));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedQueryable ThenBy(this IOrderedQueryable query,
                                           string selector,
                                           bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(query.ElementType);

        return (IOrderedQueryable)query.Provider.CreateQuery(
            CreateOrderClause(target, query.Expression, selector, false, descending));
    }

    /// <summary>
    /// Create a dynamic order by clause for a given property selector.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="descending">True to sort descending, otherwise false.</param>
    /// <returns>The sorted query.</returns>
    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query,
                                                 string selector,
                                                 bool descending = false)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        var target = Expression.Parameter(typeof(T));

        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(
            CreateOrderClause(target, query.Expression, selector, false, descending));
    }

    private static MethodCallExpression CreateOrderClause(ParameterExpression target,
                                                          Expression expression,
                                                          string selector,
                                                          bool initial,
                                                          bool descending)
    {
        var keySelector = Expression.Lambda(DynamicExpression.CreateMemberAccess(target, selector), target);

        var method = initial ? (descending ? nameof(Queryable.OrderByDescending)
                                           : nameof(Queryable.OrderBy))
                             : (descending ? nameof(Queryable.ThenByDescending)
                                           : nameof(Queryable.ThenBy));

        return Expression.Call(typeof(Queryable), method, [target.Type, keySelector.ReturnType],
            expression, Expression.Quote(keySelector));
    }

    private static MethodCallExpression CreateWhereClause(ParameterExpression target,
                                                          Expression expression,
                                                          Expression comparison)
    {
        var predicate = Expression.Lambda(comparison, target);

        return Expression.Call(typeof(Queryable), nameof(Queryable.Where), [target.Type],
            expression, Expression.Quote(predicate));
    }
}
