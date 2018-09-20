using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for async query provider.
    /// </summary>
    public class RewriteAsyncQueryProvider : IAsyncQueryProvider
    {
        /// <summary>
        /// Actual query provider.
        /// </summary>
        public IAsyncQueryProvider Provider { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public ExpressionVisitor Rewriter { get; }

        /// <summary>
        /// Create a new rewrite query provider.
        /// </summary>
        /// <param name="provider">The actual query provider.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteAsyncQueryProvider(IAsyncQueryProvider provider, ExpressionVisitor rewriter)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            Provider = provider;
            Rewriter = rewriter;
        }

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        public virtual IAsyncQueryable<TElement> RewriteQuery<TElement>(Expression expression)
        {
            // create query with now (!) rewritten expression
            return Provider.CreateQuery<TElement>(Rewriter.Visit(expression));
        }

        /// <inheritdoc />
        public virtual IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var queryable = Provider.CreateQuery<TElement>(expression);
            return new RewriteAsyncQueryable<TElement>(queryable, this);
        }

        /// <inheritdoc />
        public virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            // execute query with rewritten expression
            return Provider.ExecuteAsync<TResult>(Rewriter.Visit(expression), token);
        }
    }
}
