using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    /// <summary>
    /// Asyn enumerator for non async queries.
    /// </summary>
    public abstract class RewriteQueryEnumerator : IDbAsyncEnumerator
    {
        private readonly IEnumerator enumerator;

        /// <summary>
        /// Create a async new enumerator.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        protected RewriteQueryEnumerator(IEnumerator enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public object Current
        {
            get { return enumerator.Current; }
        }

        /// <inheritdoc />
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

        /// <summary>
        /// Destroys the enumerator.
        /// </summary>
        ~RewriteQueryEnumerator()
        {
            Dispose(false);
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
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
    /// Asyn enumerator for non async queries.
    /// </summary>
    public class RewriteQueryEnumerator<T> : RewriteQueryEnumerator, IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a async new enumerator.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteQueryEnumerator(IEnumerator<T> enumerator)
            : base(enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        public new T Current
        {
            get { return enumerator.Current; }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
