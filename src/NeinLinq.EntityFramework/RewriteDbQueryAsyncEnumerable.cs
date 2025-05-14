using System.Data.Entity.Infrastructure;
using System.Runtime.CompilerServices;

namespace NeinLinq;

/// <inheritdoc />
public sealed class RewriteDbQueryAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly IDbAsyncEnumerable dbAsyncEnumerable;

    /// <summary>
    /// Creates IAsyncEnumerable from IDbAsyncEnumerable
    /// </summary>
    /// <param name="dbAsyncEnumerable"></param>
    public RewriteDbQueryAsyncEnumerable(IDbAsyncEnumerable dbAsyncEnumerable)
    {
        if (dbAsyncEnumerable is null)
            throw new ArgumentNullException(nameof(dbAsyncEnumerable));

        this.dbAsyncEnumerable = dbAsyncEnumerable;
    }

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        return enumerateAsyncEnumerableAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

        async IAsyncEnumerable<T> enumerateAsyncEnumerableAsync([EnumeratorCancellation] CancellationToken ct)
        {
            using var asyncEnumerator = dbAsyncEnumerable.GetAsyncEnumerator();

            while (await asyncEnumerator.MoveNextAsync(ct).ConfigureAwait(false))
            {
                yield return (T)asyncEnumerator.Current;
            }
        }
    }
}
