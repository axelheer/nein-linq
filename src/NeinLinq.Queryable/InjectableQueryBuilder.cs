using System;
using System.Linq;

namespace NeinLinq.Queryable
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    public static class InjectableQueryBuilder
    {
        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable ToInjectable(this IQueryable value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable ToInjectable(this IOrderedQueryable value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable<T> ToInjectable<T>(this IQueryable<T> value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable<T> ToInjectable<T>(this IOrderedQueryable<T> value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }
    }
}
