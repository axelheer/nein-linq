using System.Collections.Generic;

namespace NeinLinq
{
    public partial class RewriteQueryEnumerable<T> : IAsyncEnumerable<T>
    {
        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
