using System.Data.Entity.Infrastructure;

namespace NeinLinq;

/// <summary>
/// Proxy for query enumerator.
/// </summary>
public class RewriteDbQueryEnumerator<T> : RewriteQueryEnumerator<T>, IDbAsyncEnumerator<T>
{
    private readonly IEnumerator<T> enumerator;

    /// <summary>
    /// Create a new enumerator proxy.
    /// </summary>
    /// <param name="enumerator">The actual enumerator.</param>
    public RewriteDbQueryEnumerator(IEnumerator<T> enumerator)
        : base(enumerator)
    {
        this.enumerator = enumerator;
    }

    /// <inheritdoc />
    object? IDbAsyncEnumerator.Current
        => Current;

    /// <inheritdoc />
    public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        => Task.FromResult(enumerator.MoveNext());
}
