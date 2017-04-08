using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteEntityQueryProvider : IQueryProvider, IAsyncQueryProvider
    {
        readonly IQueryProvider provider;
        readonly ExpressionVisitor rewriter;

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
        public RewriteEntityQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            this.provider = provider;
            this.rewriter = rewriter;
        }

        /// <inheritdoc />
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            return provider.CreateQuery<TElement>(expression).Rewrite(rewriter);
        }

        /// <inheritdoc />
        public IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            return provider.CreateQuery(expression).Rewrite(rewriter);
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

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Returning that object.")]
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            // execute query with rewritten expression; async, if possible
            var asyncProvider = provider as IAsyncQueryProvider;
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync<TResult>(rewriter.Visit(expression));
            return new RewriteEntityQueryEnumerable<TResult>(provider.CreateQuery<TResult>(rewriter.Visit(expression)));
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            var asyncProvider = provider as IAsyncQueryProvider;
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync<TResult>(rewriter.Visit(expression), cancellationToken);
            return Task.FromResult(provider.Execute<TResult>(rewriter.Visit(expression)));
        }
    }
}
