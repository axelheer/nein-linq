using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq.EntityFramework
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public abstract class RewriteDbQueryEnumerator : IEnumerator, IDbAsyncEnumerator, IDisposable
    {
        readonly IEnumerator enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        protected RewriteDbQueryEnumerator(IEnumerator enumerator)
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
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <inheritdoc />
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }
    }

    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public class RewriteDbQueryEnumerator<T> : RewriteDbQueryEnumerator, IEnumerator<T>, IDbAsyncEnumerator<T>
    {
        readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteDbQueryEnumerator(IEnumerator<T> enumerator)
            : base(enumerator)
        {
            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public new T Current => enumerator.Current;
    }
}
