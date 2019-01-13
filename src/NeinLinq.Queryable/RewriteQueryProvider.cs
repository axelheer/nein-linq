﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for query provider.
    /// </summary>
    public class RewriteQueryProvider : IQueryProvider
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
        public RewriteQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
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

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        public virtual IQueryable<TElement> RewriteQuery<TElement>(Expression expression)
        {
            // create query with now (!) rewritten expression
            return Provider.CreateQuery<TElement>(Rewrite(expression));
        }

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        public virtual IQueryable RewriteQuery(Expression expression)
        {
            // create query with now (!) rewritten expression
            return Provider.CreateQuery(Rewrite(expression));
        }

        /// <inheritdoc />
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery<TElement>(expression);
            return new RewriteQueryable<TElement>(query, this);
        }

        /// <inheritdoc />
        public virtual IQueryable CreateQuery(Expression expression)
        {
            // create query and make proxy again for rewritten query chaining
            var query = Provider.CreateQuery(expression);
            return (IQueryable)Activator.CreateInstance(
                typeof(RewriteQueryable<>).MakeGenericType(query.ElementType),
                query, this);
        }

        /// <inheritdoc />
        public virtual TResult Execute<TResult>(Expression expression)
        {
            // execute query with rewritten expression
            return Provider.Execute<TResult>(Rewrite(expression));
        }

        /// <inheritdoc />
        public virtual object Execute(Expression expression)
        {
            // execute query with rewritten expression
            return Provider.Execute(Rewrite(expression));
        }
    }
}
