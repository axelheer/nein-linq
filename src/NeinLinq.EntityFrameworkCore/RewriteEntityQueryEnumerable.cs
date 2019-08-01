using System;
using System.Collections.Generic;
using System.Threading;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public class RewriteEntityQueryEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        public RewriteEntityQueryEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new RewriteEntityQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
