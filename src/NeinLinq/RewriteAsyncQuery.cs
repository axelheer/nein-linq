#if IX

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public abstract class RewriteAsyncQuery : IOrderedAsyncQueryable
    {
        readonly Type elementType;
        readonly Expression expression;
        readonly IAsyncQueryProvider provider;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        protected RewriteAsyncQuery(IAsyncQueryable query, ExpressionVisitor rewriter)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            elementType = query.ElementType;
            expression = query.Expression;

            // replace query provider for further chaining
            provider = new RewriteAsyncQueryProvider(query.Provider, rewriter);
        }

        /// <inheritdoc />
        public Type ElementType => elementType;

        /// <inheritdoc />
        public Expression Expression => expression;

        /// <inheritdoc />
        public IAsyncQueryProvider Provider => provider;
    }

    /// <summary>
    /// Proxy for rewritten async queries.
    /// </summary>
    public class RewriteAsyncQuery<T> : RewriteAsyncQuery, IOrderedAsyncQueryable<T>
    {
        readonly Lazy<IAsyncEnumerable<T>> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteAsyncQuery(IAsyncQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
            // rewrite on enumeration
            enumerable = new Lazy<IAsyncEnumerable<T>>(() =>
                query.Provider.CreateQuery<T>(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator() => enumerable.Value.GetEnumerator();
    }
}

#endif
