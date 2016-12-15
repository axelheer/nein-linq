using System;
using System.Collections;
using System.Collections.Generic;

namespace NeinLinq.EntityFramework
{
    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public abstract class RewriteDbQueryEnumerable : IEnumerable, IDisposable
    {
        readonly IEnumerable enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        protected RewriteDbQueryEnumerable(IEnumerable enumerable)
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
                var disposable = enumerable as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public class RewriteDbQueryEnumerable<T> : RewriteDbQueryEnumerable, IEnumerable<T>
    {
        readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        public RewriteDbQueryEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();
    }
}
