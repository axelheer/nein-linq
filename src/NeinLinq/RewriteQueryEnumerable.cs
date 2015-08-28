using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable RECS0001

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public abstract partial class RewriteQueryEnumerable : IEnumerable, IDisposable
    {
        readonly IEnumerable enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        protected RewriteQueryEnumerable(IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return enumerable.GetEnumerator();
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
                var disposable = enumerable as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Proxy for query enumerable.
    /// </summary>
    public partial class RewriteQueryEnumerable<T> : RewriteQueryEnumerable, IEnumerable<T>
    {
        readonly IEnumerable<T> enumerable;

        /// <summary>
        /// Create a new enumerable proxy.
        /// </summary>
        /// <param name="enumerable">The actual enumerable.</param>
        public RewriteQueryEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            this.enumerable = enumerable;
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

#pragma warning restore RECS0001
