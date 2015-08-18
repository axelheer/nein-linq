using System.Collections.Generic;

namespace NeinLinq
{
    public partial class RewriteQuery<T> : IAsyncEnumerable<T>
    {
        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetEnumerator();
            return new RewriteQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }
    }
}
