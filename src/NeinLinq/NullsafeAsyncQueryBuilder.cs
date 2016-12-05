#if IX

using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Makes a query null-safe.
    /// </summary>
    public static class NullsafeAsyncQueryBuilder
    {
        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IAsyncQueryable ToNullsafe(this IAsyncQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedAsyncQueryable ToNullsafe(this IOrderedAsyncQueryable value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IAsyncQueryable<T> ToNullsafe<T>(this IAsyncQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }

        /// <summary>
        /// Makes a query null-safe.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedAsyncQueryable<T> ToNullsafe<T>(this IOrderedAsyncQueryable<T> value)
        {
            return value.Rewrite(new NullsafeQueryRewriter());
        }
    }
}

#endif
