namespace NeinLinq;

/// <summary>
/// Proxy for rewritten async queries.
/// </summary>
public abstract class RewriteAsyncQueryable : IOrderedAsyncQueryable
{
    /// <summary>
    /// Actual query.
    /// </summary>
    public IAsyncQueryable Query { get; }

    /// <summary>
    /// Rewriter to rewrite the query.
    /// </summary>
    public RewriteAsyncQueryProvider Provider { get; }

    /// <summary>
    /// Create a new query to rewrite.
    /// </summary>
    /// <param name="query">The actual query.</param>
    /// <param name="provider">The provider to rewrite the query.</param>
    protected RewriteAsyncQueryable(IAsyncQueryable query, RewriteAsyncQueryProvider provider)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        Query = query;
        Provider = provider;
    }

    /// <inheritdoc />
    public Type ElementType
        => Query.ElementType;

    /// <inheritdoc />
    public Expression Expression
        => Query.Expression;

    /// <inheritdoc />
    IAsyncQueryProvider IAsyncQueryable.Provider
        => Provider; // replace query provider
}

/// <summary>
/// Proxy for rewritten async queries.
/// </summary>
public class RewriteAsyncQueryable<T> : RewriteAsyncQueryable, IOrderedAsyncQueryable<T>
{
    /// <summary>
    /// Create a new query to rewrite.
    /// </summary>
    /// <param name="query">The actual query.</param>
    /// <param name="provider">The provider to rewrite the query.</param>
    public RewriteAsyncQueryable(IAsyncQueryable query, RewriteAsyncQueryProvider provider)
        : base(query, provider)
    {
    }

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => Provider.RewriteQuery<T>(Expression)
            .GetAsyncEnumerator(cancellationToken); // rewrite on enumeration
}
