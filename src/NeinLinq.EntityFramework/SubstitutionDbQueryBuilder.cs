namespace NeinLinq;

/// <summary>
/// Replaces method types.
/// </summary>
public static class SubstitutionDbQueryBuilder
{
    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable ToDbSubstitution(this IQueryable value, Type from, Type to)
        => value.DbRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable ToDbSubstitution(this IOrderedQueryable value, Type from, Type to)
        => value.DbRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable<T> ToDbSubstitution<T>(this IQueryable<T> value, Type from, Type to)
        => value.DbRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable<T> ToDbSubstitution<T>(this IOrderedQueryable<T> value, Type from, Type to)
        => value.DbRewrite(new SubstitutionQueryRewriter(from, to));
}
