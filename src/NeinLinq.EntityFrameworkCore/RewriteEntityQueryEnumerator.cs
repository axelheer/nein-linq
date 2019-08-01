using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public class RewriteEntityQueryEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteEntityQueryEnumerator(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public T Current => enumerator.Current;

        /// <inheritdoc />
        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(enumerator.MoveNext());
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            enumerator.Dispose();

            return default;
        }
    }
}
