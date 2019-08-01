using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query.Internal;

#pragma warning disable EF1001 // Internal EF Core API usage.

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteEntityQueryProvider : RewriteQueryProvider, IAsyncQueryProvider
    {
        /// <summary>
        /// Create a new rewrite query provider.
        /// </summary>
        /// <param name="provider">The actual query provider.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteEntityQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
            : base(provider, rewriter)
        {
        }

        /// <inheritdoc />
        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery<TElement>(expression);
            return new RewriteEntityQueryable<TElement>(query, this);
        }

        /// <inheritdoc />
        public override IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery(expression);
            return (IQueryable)Activator.CreateInstance(
                typeof(RewriteEntityQueryable<>).MakeGenericType(query.ElementType),
                query, this);
        }

        /// <inheritdoc />
        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            // execute query with rewritten expression; async, if possible
            if (Provider is IAsyncQueryProvider asyncProvider)
                return asyncProvider.ExecuteAsync<TResult>(Rewrite(expression), cancellationToken);
            return Provider.Execute<TResult>(Rewrite(expression));
        }
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.
