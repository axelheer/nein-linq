using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    public partial class RewriteQueryEnumerator<T> : IAsyncEnumerator<T>
    {
        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }
    }
}
