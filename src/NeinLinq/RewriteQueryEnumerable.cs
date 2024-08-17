namespace NeinLinq;

/// <summary>
/// Proxy for query enumerable.
/// </summary>
public class RewriteQueryEnumerable<T> : IEnumerable<T>, IAsyncEnumerable<T>
{
    private readonly IEnumerable<T> enumerable;

    /// <summary>
    /// Create a new enumerable proxy.
    /// </summary>
    /// <param name="enumerable">The actual enumerable.</param>
    public RewriteQueryEnumerable(IEnumerable<T> enumerable)
    {
        if (enumerable is null)
            throw new ArgumentNullException(nameof(enumerable));

        this.enumerable = enumerable;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());
}
