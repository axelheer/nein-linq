using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq.EntityFramework
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public abstract class RewriteDbQuery : IOrderedQueryable, IDbAsyncEnumerable
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
        protected RewriteDbQuery(IQueryable query, ExpressionVisitor rewriter)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            elementType = query.ElementType;
            expression = query.Expression;

            // replace query provider for further chaining
            provider = new RewriteDbQueryProvider(query.Provider, rewriter);

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

        /// <inheritdoc />
        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return (RewriteDbQueryEnumerator)Activator.CreateInstance(
                    typeof(RewriteDbQueryEnumerator<>).MakeGenericType(elementType),
                    enumerable.Value.GetEnumerator());
        }
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteDbQuery<T> : RewriteDbQuery, IOrderedQueryable<T>, IDbAsyncEnumerable<T>
    {
        readonly Lazy<IEnumerable<T>> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteDbQuery(IQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
            // rewrite on enumeration
            enumerable = new Lazy<IEnumerable<T>>(() =>
                query.Provider.CreateQuery<T>(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator() => enumerable.Value.GetEnumerator();

        /// <inheritdoc />
        public new IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteDbQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }
    }
}
