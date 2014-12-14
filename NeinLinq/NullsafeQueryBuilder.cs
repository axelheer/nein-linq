using System;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Makes queries a bit more nullsafe.
    /// </summary>
    public static class NullsafeQueryBuilder
    {
        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable ToNullsafe(this IQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable ToNullsafe(this IOrderedQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable<T> ToNullsafe<T>(this IQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query a bit more nullsafe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable<T> ToNullsafe<T>(this IOrderedQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }
    }
}
