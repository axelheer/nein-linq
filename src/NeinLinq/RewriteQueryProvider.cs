using System;
using System.Linq;
using System.Linq.Expressions;

#if EF6

using System.Data.Entity.Infrastructure;

#elif EF7

using Microsoft.Data.Entity.Query;
using System.Collections.Generic;

#endif

#if EF6 || EF7

using System.Threading;
using System.Threading.Tasks;

#endif

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteQueryProvider : IQueryProvider
#if EF6
        , IDbAsyncQueryProvider
#elif EF7
        , IEntityQueryProvider
#endif
    {
        private readonly IQueryProvider provider;
        private readonly ExpressionVisitor rewriter;

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

#if EF6

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            var asyncProvider = provider as IDbAsyncQueryProvider;
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync<TResult>(rewriter.Visit(expression), cancellationToken);
            return Task.FromResult(provider.Execute<TResult>(rewriter.Visit(expression)));
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            var asyncProvider = provider as IDbAsyncQueryProvider;
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync(rewriter.Visit(expression), cancellationToken);
            return Task.FromResult(provider.Execute(rewriter.Visit(expression)));
        }

#elif EF7

        /// <inheritdoc />
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            // execute query with rewritten expression; async, if possible
            var asyncProvider = provider as IAsyncQueryProvider;
            if (asyncProvider != null)
                return asyncProvider.ExecuteAsync<TResult>(rewriter.Visit(expression));
            return new EntityQueryable<TResult>(this, rewriter.Visit(expression));
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

#endif
    }
}
