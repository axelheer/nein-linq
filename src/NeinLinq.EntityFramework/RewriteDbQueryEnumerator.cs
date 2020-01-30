﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query enumerator.
    /// </summary>
    public class RewriteDbQueryEnumerator<T> : RewriteQueryEnumerator<T>, IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        /// <summary>
        /// Create a new enumerator proxy.
        /// </summary>
        /// <param name="enumerator">The actual enumerator.</param>
        public RewriteDbQueryEnumerator(IEnumerator<T> enumerator)
            : base(enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));

            this.enumerator = enumerator;
        }

        /// <inheritdoc />
        object? IDbAsyncEnumerator.Current => Current;

        /// <inheritdoc />
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }
    }
}
