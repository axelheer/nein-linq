using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#if EF6

using System.Data.Entity.Infrastructure;

#endif

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public abstract class RewriteQuery : IOrderedQueryable
#if EF6
        , IDbAsyncEnumerable
#endif
    {
        private readonly Type elementType;
        private readonly Expression expression;
        private readonly IQueryProvider provider;
        private readonly Lazy<IEnumerable> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        protected RewriteQuery(IQueryable query, ExpressionVisitor rewriter)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            elementType = query.ElementType;
            expression = query.Expression;

            // replace query provider for further chaining
            provider = new RewriteQueryProvider(query.Provider, rewriter);

            // rewrite on enumeration
            enumerable = new Lazy<IEnumerable>(() =>
                query.Provider.CreateQuery(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }

#if EF6

        /// <inheritdoc />
        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return (RewriteQueryEnumerator)Activator.CreateInstance(
                    typeof(RewriteQueryEnumerator<>).MakeGenericType(elementType),
                    enumerable.Value.GetEnumerator());
        }

#endif

        /// <inheritdoc />
        public Type ElementType
        {
            get { return elementType; }
        }

        /// <inheritdoc />
        public Expression Expression
        {
            get { return expression; }
        }

        /// <inheritdoc />
        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteQuery<T> : RewriteQuery, IOrderedQueryable<T>
#if EF6
        , IDbAsyncEnumerable<T>
#elif EF7
        , IAsyncEnumerable<T>
#endif
    {
        private readonly Lazy<IEnumerable<T>> enumerable;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteQuery(IQueryable<T> query, ExpressionVisitor rewriter)
            : base(query, rewriter)
        {
            // rewrite on enumeration
            enumerable = new Lazy<IEnumerable<T>>(() =>
                query.Provider.CreateQuery<T>(rewriter.Visit(query.Expression)));
        }

        /// <inheritdoc />
        public new IEnumerator<T> GetEnumerator()
        {
            return enumerable.Value.GetEnumerator();
        }

#if EF6

        /// <inheritdoc />
        public new IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }

#elif EF7

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetEnumerator();
            return new RewriteQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }

#endif
    }
}
