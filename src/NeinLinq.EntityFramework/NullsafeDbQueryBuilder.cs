using System.Linq;

namespace NeinLinq.EntityFramework
{
    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    public static class NullsafeDbQueryBuilder
    {
        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable ToNullsafe(this IQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable ToNullsafe(this IOrderedQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable<T> ToNullsafe<T>(this IQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable<T> ToNullsafe<T>(this IOrderedQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }
    }
}
