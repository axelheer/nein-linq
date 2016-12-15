using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public abstract class RewriteEntityQuery : IOrderedQueryable
    {
        readonly Type elementType;
        readonly Expression expression;
        readonly IQueryProvider provider;
        readonly Lazy<IEnumerable> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        protected RewriteEntityQuery(IQueryable query, ExpressionVisitor rewriter)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            elementType = query.ElementType;
            expression = query.Expression;

            // replace query provider for further chaining
            provider = new RewriteEntityQueryProvider(query.Provider, rewriter);

            // rewrite on enumeration
            enumerable = new Lazy<IEnumerable>(() =>
                query.Provider.CreateQuery(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator() => enumerable.Value.GetEnumerator();

        /// <inheritdoc />
        public Type ElementType => elementType;

        /// <inheritdoc />
        public Expression Expression => expression;

        /// <inheritdoc />
        public IQueryProvider Provider => provider;
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteEntityQuery<T> : RewriteEntityQuery, IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        readonly Lazy<IEnumerable<T>> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteEntityQuery(IQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
            // rewrite on enumeration
            enumerable = new Lazy<IEnumerable<T>>(() =>
                query.Provider.CreateQuery<T>(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator() => enumerable.Value.GetEnumerator();

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetEnumerator();
            return new RewriteEntityQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }
    }
}
