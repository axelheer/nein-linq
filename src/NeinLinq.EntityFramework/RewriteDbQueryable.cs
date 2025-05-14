using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NeinLinq;

/// <summary>
/// Proxy for rewritten queries.
/// </summary>
public class RewriteDbQueryable<T> : RewriteQueryable<T>, IDbAsyncEnumerable<T>, IAsyncEnumerable<T>
{
    /// <summary>
    /// Create a new query to rewrite.
    /// </summary>
    /// <param name="query">The actual query.</param>
    /// <param name="provider">The provider to rewrite the query.</param>
    public RewriteDbQueryable(IQueryable query, RewriteQueryProvider provider)
        : base(query, provider)
    {
    }

    /// <summary>
    /// Proxy for un-trackable queries.
    /// </summary>
    /// <returns>The un-trackable query.</returns>
    public IQueryable<T> AsNoTracking()
        => new RewriteDbQueryable<T>(Query.AsNoTracking(), Provider);

    /// <summary>
    /// Proxy for includeable queries.
    /// </summary>
    /// <param name="path">The path to include.</param>
    /// <returns>The includeable query.</returns>
    public IQueryable<T> Include(string path)
        => new RewriteDbQueryable<T>(Provider.RewriteQuery(Expression).Include(path), Provider);

    /// <inheritdoc />
    public IDbAsyncEnumerator<T> GetAsyncEnumerator()
    {
        // rewrite on enumeration
        var enumerable = Provider.RewriteQuery<T>(Expression);
        return enumerable is IDbAsyncEnumerable<T> asyncEnumerable
            ? asyncEnumerable.GetAsyncEnumerator()
            : new RewriteDbQueryEnumerator<T>(enumerable.GetEnumerator());
    }

    /// <inheritdoc />
    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        => GetAsyncEnumerator();

    /// <inheritdoc />
    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        // rewrite on enumeration
        var queryable = Provider.RewriteQuery<T>(Expression);

        if (queryable is IDbAsyncEnumerable dbAsyncEnumerable)
            return new RewriteDbQueryAsyncEnumerable<T>(dbAsyncEnumerable).GetAsyncEnumerator(cancellationToken);

        if (queryable is IAsyncEnumerable<T> asyncEnumerable)
            return asyncEnumerable.GetAsyncEnumerator(cancellationToken);

        return new RewriteQueryEnumerator<T>(queryable.GetEnumerator());
    }
}
