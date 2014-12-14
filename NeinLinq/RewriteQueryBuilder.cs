using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Creates rewritten queries.
    /// </summary>
    /// <remarks>
    /// Use specialized builders like <see cref="InjectableQueryBuilder"/>, <see cref="NullsafeQueryBuilder"/> or <see cref="SubstitutionQueryBuilder"/>.
    /// </remarks>
    public static class RewriteQueryBuilder
    {
        /// <summary>
        /// Rewrites a given query.
        /// </summary>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IQueryable Rewrite(this IQueryable value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

            return (IQueryable)Activator.CreateInstance(
                typeof(RewriteQuery<>).MakeGenericType(value.ElementType),
                value, rewriter);
        }

        /// <summary>
        /// Rewrites a given query.
        /// </summary>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IOrderedQueryable Rewrite(this IOrderedQueryable value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

            return (IOrderedQueryable)Activator.CreateInstance(
                typeof(RewriteQuery<>).MakeGenericType(value.ElementType),
                value, rewriter);
        }

        /// <summary>
        /// Rewrites a given query.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IQueryable<T> Rewrite<T>(this IQueryable<T> value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

            return new RewriteQuery<T>(value, rewriter);
        }

        /// <summary>
        /// Rewrites a given query.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">The query to rewrite.</param>
        /// <param name="rewriter">The rewriter to rewrite the query.</param>
        /// <returns>The rewritten query.</returns>
        public static IOrderedQueryable<T> Rewrite<T>(this IOrderedQueryable<T> value, ExpressionVisitor rewriter)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (rewriter == null)
                throw new ArgumentNullException("rewriter");

            return new RewriteQuery<T>(value, rewriter);
        }
    }
}
