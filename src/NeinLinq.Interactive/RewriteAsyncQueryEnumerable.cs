using System;
using System.Collections.Generic;

namespace NeinLinq.Interactive
{
    /// <summary>
    /// Proxy for async query enumerable.
    /// </summary>
    public class RewriteAsyncQueryEnumerable<T> : IAsyncEnumerable<T>, IDisposable
    {
        readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        public RewriteAsyncQueryEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new RewriteAsyncQueryEnumerator<T>(enumerable.GetEnumerator());
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the resources (other than memory).
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (enumerable is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}
