using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Create rewritten queries.
    /// </summary>
    public static class RewriteEntityQueryBuilder
    {
        /// <summary>
        /// Rewrite a given query.
        /// </summary>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IQueryable Rewrite(this IQueryable value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

            return (IQueryable)Activator.CreateInstance(
                typeof(RewriteEntityQueryable<>).MakeGenericType(value.ElementType),
                provider, value);
        }

        /// <summary>
        /// Rewrite a given query.
        /// </summary>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IOrderedQueryable Rewrite(this IOrderedQueryable value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

            return (IOrderedQueryable)Activator.CreateInstance(
                typeof(RewriteEntityQueryable<>).MakeGenericType(value.ElementType),
                provider, value);
        }

        /// <summary>
        /// Rewrite a given query.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IQueryable<T> Rewrite<T>(this IQueryable<T> value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

            return new RewriteEntityQueryable<T>(provider, value);
        }

        /// <summary>
        /// Rewrite a given query.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IOrderedQueryable<T> Rewrite<T>(this IOrderedQueryable<T> value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            var provider = new RewriteEntityQueryProvider(value.Provider, rewriter);

            return new RewriteEntityQueryable<T>(provider, value);
        }
    }
}
