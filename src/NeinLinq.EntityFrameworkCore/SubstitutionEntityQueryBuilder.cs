namespace NeinLinq;

/// <summary>
/// Replaces method types.
/// </summary>
public static class SubstitutionEntityQueryBuilder
{
    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable ToEntitySubstitution(this IQueryable value, Type from, Type to)
        => value.EntityRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable ToEntitySubstitution(this IOrderedQueryable value, Type from, Type to)
        => value.EntityRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable<T> ToEntitySubstitution<T>(this IQueryable<T> value, Type from, Type to)
        => value.EntityRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable<T> ToEntitySubstitution<T>(this IOrderedQueryable<T> value, Type from, Type to)
        => value.EntityRewrite(new SubstitutionQueryRewriter(from, to));
}
