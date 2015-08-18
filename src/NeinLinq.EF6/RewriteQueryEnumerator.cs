using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    public abstract partial class RewriteQueryEnumerator : IDbAsyncEnumerator
    {
        /// <inheritdoc />
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }
    }

    public partial class RewriteQueryEnumerator<T> : IDbAsyncEnumerator<T>
    {
    }
}
