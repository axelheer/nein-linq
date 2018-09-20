using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public class RewriteAsyncQueryable<T> : IOrderedAsyncQueryable<T>
    {
        private readonly IAsyncQueryable queryable;
        private readonly RewriteAsyncQueryProvider provider;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteAsyncQueryable(IAsyncQueryable queryable, RewriteAsyncQueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            this.queryable = queryable;
            this.provider = provider;
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator()
        {
            // rewrite on enumeration
            return provider.RewriteQuery<T>(Expression).GetEnumerator();
        }

        /// <inheritdoc />
        public Type ElementType => queryable.ElementType;

        /// <inheritdoc />
        public Expression Expression => queryable.Expression;

        /// <inheritdoc />
        public IAsyncQueryProvider Provider => provider; // replace query provider
    }
}
