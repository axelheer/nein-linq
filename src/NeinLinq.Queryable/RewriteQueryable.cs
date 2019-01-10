using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public abstract class RewriteQueryable : IOrderedQueryable
    {
        /// <summary>
        /// Actual query-able.
        /// </summary>
        public IQueryable Queryable { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public RewriteQueryProvider Provider { get; }

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        protected RewriteQueryable(IQueryable queryable, RewriteQueryProvider provider)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            Queryable = queryable;
            Provider = provider;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery(Expression).GetEnumerator(); 
        }

        /// <inheritdoc />
        public Type ElementType => Queryable.ElementType;

        /// <inheritdoc />
        public Expression Expression => Queryable.Expression;

        /// <inheritdoc />
        IQueryProvider IQueryable.Provider => Provider; // replace query provider
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteQueryable<T> : RewriteQueryable, IOrderedQueryable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteQueryable(IQueryable<T> queryable, RewriteQueryProvider provider)
            : base(queryable, provider)
        {
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery<T>(Expression).GetEnumerator();
        }
    }
}
