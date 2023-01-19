namespace NeinLinq;

/// <summary>
/// Create rewritten async queries.
/// </summary>
public static class RewriteAsyncQueryBuilder
{
    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IAsyncQueryable AsyncRewrite(this IAsyncQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteAsyncQueryProvider(value.Provider, rewriter);

        return (IAsyncQueryable)Activator.CreateInstance(
            typeof(RewriteAsyncQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedAsyncQueryable AsyncRewrite(this IOrderedAsyncQueryable value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteAsyncQueryProvider(value.Provider, rewriter);

        return (IOrderedAsyncQueryable)Activator.CreateInstance(
            typeof(RewriteAsyncQueryable<>).MakeGenericType(value.ElementType),
            value, provider)!;
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IAsyncQueryable<T> AsyncRewrite<T>(this IAsyncQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteAsyncQueryProvider(value.Provider, rewriter);

        return new RewriteAsyncQueryable<T>(value, provider);
    }

    /// <summary>
    /// Rewrite a given query.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">The query to rewrite.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    /// <returns>The rewritten query.</returns>
    public static IOrderedAsyncQueryable<T> AsyncRewrite<T>(this IOrderedAsyncQueryable<T> value, ExpressionVisitor rewriter)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var provider = new RewriteAsyncQueryProvider(value.Provider, rewriter);

        return new RewriteAsyncQueryable<T>(value, provider);
    }
}
