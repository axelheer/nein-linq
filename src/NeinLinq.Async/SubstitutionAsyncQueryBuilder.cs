namespace NeinLinq;

/// <summary>
/// Replaces method types.
/// </summary>
public static class SubstitutionAsyncQueryBuilder
{
    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable ToAsyncSubstitution(this IAsyncQueryable value, Type from, Type to)
        => value.AsyncRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable ToAsyncSubstitution(this IOrderedAsyncQueryable value, Type from, Type to)
        => value.AsyncRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable<T> ToAsyncSubstitution<T>(this IAsyncQueryable<T> value, Type from, Type to)
        => value.AsyncRewrite(new SubstitutionQueryRewriter(from, to));

    /// <summary>
    /// Replaces methods of type <c>from</c> with methods of type <c>to</c>.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable<T> ToAsyncSubstitution<T>(this IOrderedAsyncQueryable<T> value, Type from, Type to)
        => value.AsyncRewrite(new SubstitutionQueryRewriter(from, to));
}
