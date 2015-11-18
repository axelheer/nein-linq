using Microsoft.Data.Entity.Query.Internal;
using System.Collections.Generic;

namespace NeinLinq
{
    public partial class RewriteQuery<T> : IAsyncEnumerable<T>
    {
        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return ((IAsyncQueryProvider)Provider).ExecuteAsync<T>(Expression).GetEnumerator();
        }
    }
}
