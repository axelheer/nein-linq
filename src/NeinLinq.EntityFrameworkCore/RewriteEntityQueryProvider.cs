using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

#pragma warning disable EF1001

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
                query, this)!;
        }

        /// <inheritdoc />
        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            // execute query with rewritten expression; async, if possible
            return Provider is IAsyncQueryProvider asyncProvider
                ? asyncProvider.ExecuteAsync<TResult>(Rewrite(expression), cancellationToken)
                : typeof(Task).IsAssignableFrom(typeof(TResult))
                ? Execute<TResult>(ExecuteTaskMethod, expression)
                : Provider.Execute<TResult>(Rewrite(expression));
        }

        private TResult Execute<TResult>(MethodInfo method, Expression expression)
        {
            return (TResult)(method.MakeGenericMethod(typeof(TResult).GetGenericArguments()[0])
                .Invoke(this, new object[] { expression })!);
        }

        private static readonly MethodInfo ExecuteTaskMethod
            = typeof(RewriteEntityQueryProvider).GetMethod(nameof(ExecuteTask), BindingFlags.Instance | BindingFlags.NonPublic)!;

        private Task<TResult> ExecuteTask<TResult>(Expression expression)
            => Task.FromResult(Provider.Execute<TResult>(Rewrite(expression)));
    }
}
