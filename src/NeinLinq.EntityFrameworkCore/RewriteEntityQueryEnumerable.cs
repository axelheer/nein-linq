using System;
using System.Collections;
using System.Collections.Generic;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public abstract class RewriteEntityQueryEnumerable : IEnumerable, IDisposable
    {
        readonly IEnumerable enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        protected RewriteEntityQueryEnumerable(IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator() => enumerable.GetEnumerator();

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

    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public class RewriteEntityQueryEnumerable<T> : RewriteEntityQueryEnumerable, IEnumerable<T>, IAsyncEnumerable<T>
    {
        readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        public RewriteEntityQueryEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return new RewriteEntityQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
