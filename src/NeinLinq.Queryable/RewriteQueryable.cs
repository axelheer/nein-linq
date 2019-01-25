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
        /// Actual query.
        /// </summary>
        public IQueryable Query { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public IRewriteQueryProvider Provider { get; }

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        protected RewriteQueryable(IQueryable query, IRewriteQueryProvider provider)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            Query = query;
            Provider = provider;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery(Expression).GetEnumerator(); 
        }

        /// <inheritdoc />
        public Type ElementType => Query.ElementType;

        /// <inheritdoc />
        public Expression Expression => Query.Expression;

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
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteQueryable(IQueryable query, IRewriteQueryProvider provider)
            : base(query, provider)
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
