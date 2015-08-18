using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable RECS0001

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public abstract partial class RewriteQueryEnumerator : IEnumerator, IDisposable
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
    public partial class RewriteQueryEnumerator<T> : RewriteQueryEnumerator, IEnumerator<T>
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

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

#pragma warning restore RECS0001
