using System;
using System.Collections;
using System.Collections.Generic;

#if EF6

using System.Data.Entity.Infrastructure;

#endif

#if EF6 || EF7

using System.Threading;
using System.Threading.Tasks;

#endif

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public abstract class RewriteQueryEnumerator : IEnumerator, IDisposable
#if EF6
        , IDbAsyncEnumerator
#endif
    {
        readonly IEnumerator enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        protected RewriteQueryEnumerator(IEnumerator enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public object Current
        {
            get { return enumerator.Current; }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

#if EF6

        /// <inheritdoc />
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

#endif

        /// <inheritdoc />
        public void Reset()
        {
            enumerator.Reset();
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
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public class RewriteQueryEnumerator<T> : RewriteQueryEnumerator, IEnumerator<T>
#if EF6
        , IDbAsyncEnumerator<T>
#elif EF7
        , IAsyncEnumerator<T>
#endif
    {
        readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteQueryEnumerator(IEnumerator<T> enumerator)
            : base(enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public new T Current
        {
            get { return enumerator.Current; }
        }

#if EF7

        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

#endif

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
