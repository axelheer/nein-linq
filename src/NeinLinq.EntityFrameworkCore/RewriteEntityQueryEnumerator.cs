using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public abstract class RewriteEntityQueryEnumerator : IEnumerator, IDisposable
    {
        readonly IEnumerator enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        protected RewriteEntityQueryEnumerator(IEnumerator enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public object Current => enumerator.Current;

        /// <inheritdoc />
        public bool MoveNext() => enumerator.MoveNext();

        /// <inheritdoc />
        public void Reset() => enumerator.Reset();

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
                if (enumerator is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public class RewriteEntityQueryEnumerator<T> : RewriteEntityQueryEnumerator, IEnumerator<T>, IAsyncEnumerator<T>
    {
        readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteEntityQueryEnumerator(IEnumerator<T> enumerator)
            : base(enumerator)
        {
            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public new T Current => enumerator.Current;

        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
