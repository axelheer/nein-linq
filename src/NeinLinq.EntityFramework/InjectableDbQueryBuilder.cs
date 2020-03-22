using System;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    public static class InjectableDbQueryBuilder
    {
        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable ToDbInjectable(this IQueryable value, params Type[] whitelist)
            => value.DbRewrite(new InjectableQueryRewriter(whitelist));

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable ToDbInjectable(this IOrderedQueryable value, params Type[] whitelist)
            => value.DbRewrite(new InjectableQueryRewriter(whitelist));

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IQueryable<T> ToDbInjectable<T>(this IQueryable<T> value, params Type[] whitelist)
            => value.DbRewrite(new InjectableQueryRewriter(whitelist));

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedQueryable<T> ToDbInjectable<T>(this IOrderedQueryable<T> value, params Type[] whitelist)
            => value.DbRewrite(new InjectableQueryRewriter(whitelist));
    }
}
