using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteDbQueryProvider : RewriteQueryProvider, IDbAsyncQueryProvider
    {
        /// <summary>
        /// Create a new rewrite query provider.
        /// </summary>
        /// <param name="provider">The actual query provider.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteDbQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
            : base(provider, rewriter)
        {
        }

        /// <inheritdoc />
        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery<TElement>(expression);
            return new RewriteDbQueryable<TElement>(query, this);
        }

        /// <inheritdoc />
        public override IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery(expression);
            return (IQueryable?)Activator.CreateInstance(
                typeof(RewriteDbQueryable<>).MakeGenericType(query.ElementType),
                query, this) ?? throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            return Provider is IDbAsyncQueryProvider asyncProvider
                ? asyncProvider.ExecuteAsync<TResult>(Rewrite(expression), cancellationToken)
                : Task.FromResult(Provider.Execute<TResult>(Rewrite(expression)));
        }

        /// <inheritdoc />
        public virtual Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            return Provider is IDbAsyncQueryProvider asyncProvider
                ? asyncProvider.ExecuteAsync(Rewrite(expression), cancellationToken)
                : Task.FromResult(Provider.Execute(Rewrite(expression)));
        }
    }
}
