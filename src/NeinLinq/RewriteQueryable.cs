namespace NeinLinq;

#pragma warning disable CA1010

/// <summary>
/// Proxy for rewritten queries.
/// </summary>
public abstract class RewriteQueryable : IOrderedQueryable
{
    /// <summary>
    /// Actual query.
    /// </summary>
    public IQueryable Query { get; }

    /// <summary>
    /// Rewriter to rewrite the query.
    /// </summary>
    public RewriteQueryProvider Provider { get; }

    /// <summary>
    /// Create a new query to rewrite.
    /// </summary>
    /// <param name="query">The actual query.</param>
    /// <param name="provider">The provider to rewrite the query.</param>
    protected RewriteQueryable(IQueryable query, RewriteQueryProvider provider)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        Query = query;
        Provider = provider;
    }

    /// <inheritdoc />
    public IEnumerator GetEnumerator()
        => Provider.RewriteQuery(Expression)
            .GetEnumerator(); // rewrite on enumeration

    /// <inheritdoc />
    public Type ElementType
        => Query.ElementType;

    /// <inheritdoc />
    public Expression Expression
        => Query.Expression;

    /// <inheritdoc />
    IQueryProvider IQueryable.Provider
        => Provider; // replace query provider
}

#pragma warning restore CA1010

/// <summary>
/// Proxy for rewritten queries.
/// </summary>
public class RewriteQueryable<T> : RewriteQueryable, IOrderedQueryable<T>, IAsyncEnumerable<T>
{
    /// <summary>
    /// Create a new query to rewrite.
    /// </summary>
    /// <param name="query">The actual query.</param>
    /// <param name="provider">The provider to rewrite the query.</param>
    public RewriteQueryable(IQueryable query, RewriteQueryProvider provider)
        : base(query, provider)
    {
    }

    /// <summary>
    /// Gets a debug representation of the underlying query.
    /// </summary>
    public string? DebugString
        => Provider.RewriteQuery<T>(Expression).ToString();

    /// <inheritdoc />
    public new IEnumerator<T> GetEnumerator()
        => Provider.RewriteQuery<T>(Expression)
            .GetEnumerator(); // rewrite on enumeration

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        // rewrite on enumeration
        var enumerable = Provider.RewriteQuery<T>(Expression);
        return enumerable is IAsyncEnumerable<T> asyncEnumerable
            ? asyncEnumerable.GetAsyncEnumerator(cancellationToken)
            : new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());
    }
}
