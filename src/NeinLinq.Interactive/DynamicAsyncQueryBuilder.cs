using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq.Interactive
{
    /// <summary>
    /// Helps building dynamic async queries.
    /// </summary>
    public static class DynamicAsyncQueryBuilder
    {
        /// <summary>
        /// Create a dynamic where clause for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to filter.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The filtered query.</returns>
        public static IAsyncQueryable<T> Where<T>(this IAsyncQueryable<T> query, string selector, DynamicCompare comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
                throw new ArgumentOutOfRangeException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return query.Provider.CreateQuery<T>(CreateAsyncWhereClause(target, query.Expression, selector, comparer, value));
        }

        /// <summary>
        /// Create a dynamic where clause for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to filter.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The filtered query.</returns>
        public static IAsyncQueryable<T> Where<T>(this IAsyncQueryable<T> query, string selector, string comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrEmpty(comparer))
                throw new ArgumentNullException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return query.Provider.CreateQuery<T>(CreateAsyncWhereClause(target, query.Expression, selector, comparer, value));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedAsyncQueryable<T> OrderBy<T>(this IAsyncQueryable<T> query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(typeof(T));
            var method = descending ? nameof(System.Linq.AsyncQueryable.OrderByDescending) : nameof(System.Linq.AsyncQueryable.OrderBy);

            return (IOrderedAsyncQueryable<T>)query.Provider.CreateQuery<T>(CreateAsyncOrderClause(target, query.Expression, selector, method));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedAsyncQueryable<T> ThenBy<T>(this IOrderedAsyncQueryable<T> query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(typeof(T));
            var method = descending ? nameof(System.Linq.AsyncQueryable.ThenByDescending) : nameof(System.Linq.AsyncQueryable.ThenBy);

            return (IOrderedAsyncQueryable<T>)query.Provider.CreateQuery<T>(CreateAsyncOrderClause(target, query.Expression, selector, method));
        }

        static Expression CreateAsyncOrderClause(ParameterExpression target, Expression expression, string selector, string method)
        {
            var keySelector = Expression.Lambda(DynamicQuery.CreateMemberAccess(target, selector), target);

            return Expression.Call(typeof(System.Linq.AsyncQueryable), method, new[] { target.Type, keySelector.ReturnType },
                expression, Expression.Quote(keySelector));
        }

        static Expression CreateAsyncWhereClause(ParameterExpression target, Expression expression, string selector, DynamicCompare comparer, string value)
        {
            var predicate = Expression.Lambda(DynamicQuery.CreateComparison(target, selector, comparer, value), target);

            return Expression.Call(typeof(System.Linq.AsyncQueryable), nameof(System.Linq.AsyncQueryable.Where), new[] { target.Type },
                expression, Expression.Quote(predicate));
        }

        static Expression CreateAsyncWhereClause(ParameterExpression target, Expression expression, string selector, string comparer, string value)
        {
            var predicate = Expression.Lambda(DynamicQuery.CreateComparison(target, selector, comparer, value), target);

            return Expression.Call(typeof(System.Linq.AsyncQueryable), nameof(System.Linq.AsyncQueryable.Where), new[] { target.Type },
                expression, Expression.Quote(predicate));
        }
    }
}
