using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq.Interactive
{
    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public class RewriteAsyncQueryable<T> : IOrderedAsyncQueryable<T>
    {
        private readonly RewriteAsyncQueryProvider provider;
        private readonly IAsyncQueryable queryable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="provider">The provider to rewrite the query.</param>
        /// <param name="queryable">The actual query.</param>
        public RewriteAsyncQueryable(RewriteAsyncQueryProvider provider, IAsyncQueryable queryable)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            this.provider = provider;
            this.queryable = queryable;
        }

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <returns>A rewritten query.</returns>
        public IAsyncQueryable<T> Rewrite()
        {
            var expression = provider.Rewriter.Visit(queryable.Expression);
            return queryable.Provider.CreateQuery<T>(expression);
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator()
        {
            // rewrite on enumeration
            var enumerable = Rewrite();
            return enumerable.GetEnumerator();
        }

        /// <inheritdoc />
        public Type ElementType => queryable.ElementType;

        /// <inheritdoc />
        public Expression Expression => queryable.Expression;

        /// <inheritdoc />
        public IAsyncQueryProvider Provider => provider; // replace query provider
    }
}
