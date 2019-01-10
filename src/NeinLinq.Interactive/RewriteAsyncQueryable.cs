using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public abstract class RewriteAsyncQueryable : IOrderedAsyncQueryable
    {
        /// <summary>
        /// Actual query-able.
        /// </summary>
        public IAsyncQueryable Queryable { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public RewriteAsyncQueryProvider Provider { get; }

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        protected RewriteAsyncQueryable(IAsyncQueryable queryable, RewriteAsyncQueryProvider provider)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            Queryable = queryable;
            Provider = provider;
        }

        /// <inheritdoc />
        public Type ElementType => Queryable.ElementType;

        /// <inheritdoc />
        public Expression Expression => Queryable.Expression;

        /// <inheritdoc />
        IAsyncQueryProvider IAsyncQueryable.Provider => Provider; // replace query provider
    }

    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public class RewriteAsyncQueryable<T> : RewriteAsyncQueryable, IOrderedAsyncQueryable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteAsyncQueryable(IAsyncQueryable<T> queryable, RewriteAsyncQueryProvider provider)
            : base(queryable, provider)
        {
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery<T>(Expression).GetEnumerator();
        }
    }
}
