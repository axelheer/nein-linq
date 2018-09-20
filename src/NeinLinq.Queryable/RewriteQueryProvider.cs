using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider provider;
        private readonly ExpressionVisitor rewriter;

        /// <summary>
        /// Actual query provider.
        /// </summary>
        public IQueryProvider Provider => provider;

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public ExpressionVisitor Rewriter => rewriter;

        /// <summary>
        /// Create a new rewrite query provider.
        /// </summary>
        /// <param name="provider">The actual query provider.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            this.provider = provider;
            this.rewriter = rewriter;
        }

        /// <inheritdoc />
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var queryable = provider.CreateQuery<TElement>(expression);
            return new RewriteQueryable<TElement>(queryable, this);
        }

        /// <inheritdoc />
        public virtual IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var queryable = provider.CreateQuery(expression);
            return (IQueryable)Activator.CreateInstance(
                typeof(RewriteQueryable<>).MakeGenericType(queryable.ElementType),
                queryable, this);
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Expression expression)
        {
            // execute query with rewritten expression
            return provider.Execute<TResult>(rewriter.Visit(expression));
        }

        /// <inheritdoc />
        public object Execute(Expression expression)
        {
            // execute query with rewritten expression
            return provider.Execute(rewriter.Visit(expression));
        }
    }
}
