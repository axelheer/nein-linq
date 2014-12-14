using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public abstract class RewriteQuery : IOrderedQueryable
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
                throw new ArgumentNullException("query");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

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
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class RewriteQuery<T> : RewriteQuery, IOrderedQueryable<T>
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
    }
}
