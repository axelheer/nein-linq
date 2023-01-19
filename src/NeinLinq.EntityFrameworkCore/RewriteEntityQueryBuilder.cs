namespace NeinLinq;

/// <summary>
/// Create rewritten queries.
/// </summary>
public static class RewriteEntityQueryBuilder
{
    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IQueryable EntityRewrite(this IQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

        return (IQueryable)Activator.CreateInstance(
            typeof(RewriteEntityQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedQueryable EntityRewrite(this IOrderedQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

        return (IOrderedQueryable)Activator.CreateInstance(
            typeof(RewriteEntityQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IQueryable<T> EntityRewrite<T>(this IQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

        return new RewriteEntityQueryable<T>(value, provider);
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedQueryable<T> EntityRewrite<T>(this IOrderedQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

        return new RewriteEntityQueryable<T>(value, provider);
    }
}
