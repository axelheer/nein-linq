using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteEntityQueryProvider : EntityQueryProvider, IRewriteQueryProvider
    {
        /// <summary>
        /// Actual query provider.
        /// </summary>
        public IQueryProvider Provider { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public ExpressionVisitor Rewriter { get; }

        /// <summary>
        /// Create a new rewrite query provider.
        /// </summary>
        /// <param name="provider">The actual query provider.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        public RewriteEntityQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
            : base(new EmptyQueryCompiler())
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            Provider = provider;
            Rewriter = rewriter;
        }

        private readonly ExpressionVisitor cleaner = new RewriteQueryCleaner();

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query expression.</returns>
        protected virtual Expression Rewrite(Expression expression)
        {
            // clean-up and rewrite the whole expression
            return Rewriter.Visit(cleaner.Visit(expression));
        }

        /// <inheritdoc />
        public virtual IQueryable<TElement> RewriteQuery<TElement>(Expression expression)
        {
            // create query with now (!) rewritten expression
            return Provider.CreateQuery<TElement>(Rewrite(expression));
        }

        /// <inheritdoc />
        public virtual IQueryable RewriteQuery(Expression expression)
        {
            // create query with now (!) rewritten expression
            return Provider.CreateQuery(Rewrite(expression));
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
        public override TResult Execute<TResult>(Expression expression)
        {
            // execute query with rewritten expression
            return Provider.Execute<TResult>(Rewrite(expression));
        }

        /// <inheritdoc />
        public override object Execute(Expression expression)
        {
            // execute query with rewritten expression
            return Provider.Execute(Rewrite(expression));
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            // execute query with rewritten expression; async, if possible
            if (Provider is IAsyncQueryProvider asyncProvider)
                return asyncProvider.ExecuteAsync<TResult>(Rewrite(expression));
            return new RewriteEntityQueryEnumerable<TResult>(Provider.CreateQuery<TResult>(Rewrite(expression)));
        }

        /// <inheritdoc />
        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            // execute query with rewritten expression; async, if possible
            if (Provider is IAsyncQueryProvider asyncProvider)
                return asyncProvider.ExecuteAsync<TResult>(Rewrite(expression), cancellationToken);
            return Task.FromResult(Provider.Execute<TResult>(Rewrite(expression)));
        }

        [ExcludeFromCodeCoverage]
        private class EmptyQueryCompiler : IQueryCompiler
        {
            [ExcludeFromCodeCoverage]
            public Func<QueryContext, IAsyncEnumerable<TResult>> CreateCompiledAsyncEnumerableQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            [ExcludeFromCodeCoverage]
            public Func<QueryContext, Task<TResult>> CreateCompiledAsyncTaskQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            [ExcludeFromCodeCoverage]
            public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            [ExcludeFromCodeCoverage]
            public TResult Execute<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            [ExcludeFromCodeCoverage]
            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            [ExcludeFromCodeCoverage]
            public Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }
        }
    }
}
