using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeinLinq
{
    public partial class RewriteQueryProvider : IDbAsyncQueryProvider
    {
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
    }
}
