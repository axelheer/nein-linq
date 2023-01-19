namespace NeinLinq;

/// <summary>
/// Create rewritten queries.
/// </summary>
public static class RewriteDbQueryBuilder
{
    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IQueryable DbRewrite(this IQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteDbQueryProvider(value.Provider, rewriter);

        return (IQueryable)Activator.CreateInstance(
            typeof(RewriteDbQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedQueryable DbRewrite(this IOrderedQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteDbQueryProvider(value.Provider, rewriter);

        return (IOrderedQueryable)Activator.CreateInstance(
            typeof(RewriteDbQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IQueryable<T> DbRewrite<T>(this IQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteDbQueryProvider(value.Provider, rewriter);

        return new RewriteDbQueryable<T>(value, provider);
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedQueryable<T> DbRewrite<T>(this IOrderedQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteDbQueryProvider(value.Provider, rewriter);

        return new RewriteDbQueryable<T>(value, provider);
    }
}
