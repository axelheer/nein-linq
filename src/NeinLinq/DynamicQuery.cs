namespace NeinLinq;

/// <summary>
/// Helps building dynamic queries.
/// </summary>
public static class DynamicQuery
{
    /// <summary>
    /// Create a dynamic predicate for a given property selector, comparison method and reference value.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The dynamic predicate.</returns>
    public static Expression<Func<T, bool>> CreatePredicate<T>(string selector,
                                                               DynamicCompare comparer,
                                                               string? value,
                                                               IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
            throw new ArgumentOutOfRangeException(nameof(comparer));

        var target = Expression.Parameter(typeof(T));

        return Expression.Lambda<Func<T, bool>>(
            DynamicExpression.CreateComparison(target, selector, comparer, value, provider), target);
    }

    /// <summary>
    /// Create a dynamic predicate for a given property selector, comparison method and reference value.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The dynamic predicate.</returns>
    public static Expression<Func<T, bool>> CreatePredicate<T>(string selector,
                                                               string comparer,
                                                               string? value,
                                                               IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (string.IsNullOrEmpty(comparer))
            throw new ArgumentNullException(nameof(comparer));

        var target = Expression.Parameter(typeof(T));

        return Expression.Lambda<Func<T, bool>>(
            DynamicExpression.CreateComparison(target, selector, comparer, value, provider), target);
    }
}
