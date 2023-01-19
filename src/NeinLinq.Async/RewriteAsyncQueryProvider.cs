namespace NeinLinq;

/// <summary>
/// Proxy for async query provider.
/// </summary>
public class RewriteAsyncQueryProvider : IAsyncQueryProvider
{
    /// <summary>
    /// Actual query provider.
    /// </summary>
    public IAsyncQueryProvider Provider { get; }

    /// <summary>
    /// Rewriter to rewrite the query.
    /// </summary>
    public ExpressionVisitor Rewriter { get; }

    /// <summary>
    /// Create a new rewrite query provider.
    /// </summary>
    /// <param name="provider">The actual query provider.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    public RewriteAsyncQueryProvider(IAsyncQueryProvider provider, ExpressionVisitor rewriter)
    {
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        Provider = provider;
        Rewriter = rewriter;
    }

    private readonly ExpressionVisitor cleaner
        = new RewriteAsyncQueryCleaner();

    /// <summary>
    /// Rewrites the entire query expression.
    /// </summary>
    /// <param name="expression">The query expression.</param>
    /// <returns>A rewritten query expression.</returns>
    protected virtual Expression Rewrite(Expression expression)
    {
        // clean-up and rewrite the whole expression
        var cleaned = cleaner.Visit(expression);
        return Rewriter.Visit(cleaned);
    }

    /// <summary>
    /// Rewrites the entire query expression.
    /// </summary>
    /// <param name="expression">The query expression.</param>
    /// <returns>A rewritten query.</returns>
    public virtual IAsyncQueryable<TElement> RewriteQuery<TElement>(Expression expression)
    {
        // create query with now (!) rewritten expression
        var rewritten = Rewrite(expression);
        return Provider.CreateQuery<TElement>(rewritten);
    }

    /// <inheritdoc />
    public virtual IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        // create query and make proxy again for rewritten query chaining
        var query = Provider.CreateQuery<TElement>(expression);
        return new RewriteAsyncQueryable<TElement>(query, this);
    }

    /// <inheritdoc />
    public virtual ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
    {
        // execute query with rewritten expression
        var rewritten = Rewrite(expression);
        return Provider.ExecuteAsync<TResult>(rewritten, token);
    }
}
