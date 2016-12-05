#if IX

using System;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    public static class InjectableAsyncQueryBuilder
    {
        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IAsyncQueryable ToInjectable(this IAsyncQueryable value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }

        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="value">A query.</param>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        /// <returns>A query proxy.</returns>
        public static IOrderedAsyncQueryable ToInjectable(this IOrderedAsyncQueryable value, params Type[] whitelist)
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
        public static IAsyncQueryable<T> ToInjectable<T>(this IAsyncQueryable<T> value, params Type[] whitelist)
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
        public static IOrderedAsyncQueryable<T> ToInjectable<T>(this IOrderedAsyncQueryable<T> value, params Type[] whitelist)
        {
            return value.Rewrite(new InjectableQueryRewriter(whitelist));
        }
    }
}

#endif
