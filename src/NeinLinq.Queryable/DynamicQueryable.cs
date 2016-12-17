using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Helps building dynamic queries.
    /// </summary>
    public static class DynamicQueryable
    {
        /// <summary>
        /// Create a dynamic where clause for a given property selector, comparison method and reference value.
        /// </summary>
        /// <param name="query">The query to filter.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable Where(this IQueryable query, string selector, DynamicCompare comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
                throw new ArgumentOutOfRangeException(nameof(comparer));

            var target = Expression.Parameter(query.ElementType);

            return query.Provider.CreateQuery(CreateWhereClause(target, query.Expression, selector, comparer, value));
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
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string selector, DynamicCompare comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
                throw new ArgumentOutOfRangeException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return query.Provider.CreateQuery<T>(CreateWhereClause(target, query.Expression, selector, comparer, value));
        }

        /// <summary>
        /// Create a dynamic where clause for a given property selector, comparison method and reference value.
        /// </summary>
        /// <param name="query">The query to filter.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable Where(this IQueryable query, string selector, string comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrEmpty(comparer))
                throw new ArgumentNullException(nameof(comparer));

            var target = Expression.Parameter(query.ElementType);

            return query.Provider.CreateQuery(CreateWhereClause(target, query.Expression, selector, comparer, value));
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
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string selector, string comparer, string value)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrEmpty(comparer))
                throw new ArgumentNullException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return query.Provider.CreateQuery<T>(CreateWhereClause(target, query.Expression, selector, comparer, value));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedQueryable OrderBy(this IQueryable query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(query.ElementType);
            var method = descending ? nameof(System.Linq.Queryable.OrderByDescending) : nameof(System.Linq.Queryable.OrderBy);

            return (IOrderedQueryable)query.Provider.CreateQuery(CreateOrderClause(target, query.Expression, selector, method));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(typeof(T));
            var method = descending ? nameof(System.Linq.Queryable.OrderByDescending) : nameof(System.Linq.Queryable.OrderBy);

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(CreateOrderClause(target, query.Expression, selector, method));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedQueryable ThenBy(this IOrderedQueryable query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(query.ElementType);
            var method = descending ? nameof(System.Linq.Queryable.ThenByDescending) : nameof(System.Linq.Queryable.ThenBy);

            return (IOrderedQueryable)query.Provider.CreateQuery(CreateOrderClause(target, query.Expression, selector, method));
        }

        /// <summary>
        /// Create a dynamic order by clause for a given property selector.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="query">The query to sort.</param>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="descending">True to sort descending, otherwise false.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string selector, bool descending = false)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));

            var target = Expression.Parameter(typeof(T));
            var method = descending ? nameof(System.Linq.Queryable.ThenByDescending) : nameof(System.Linq.Queryable.ThenBy);

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(CreateOrderClause(target, query.Expression, selector, method));
        }

        static Expression CreateOrderClause(ParameterExpression target, Expression expression, string selector, string method)
        {
            var keySelector = Expression.Lambda(DynamicQuery.CreateMemberAccess(target, selector), target);

            return Expression.Call(typeof(System.Linq.Queryable), method, new[] { target.Type, keySelector.ReturnType },
                expression, Expression.Quote(keySelector));
        }

        static Expression CreateWhereClause(ParameterExpression target, Expression expression, string selector, DynamicCompare comparer, string value)
        {
            var predicate = Expression.Lambda(DynamicQuery.CreateComparison(target, selector, comparer, value), target);

            return Expression.Call(typeof(System.Linq.Queryable), nameof(System.Linq.Queryable.Where), new[] { target.Type },
                expression, Expression.Quote(predicate));
        }

        static Expression CreateWhereClause(ParameterExpression target, Expression expression, string selector, string comparer, string value)
        {
            var predicate = Expression.Lambda(DynamicQuery.CreateComparison(target, selector, comparer, value), target);

            return Expression.Call(typeof(System.Linq.Queryable), nameof(System.Linq.Queryable.Where), new[] { target.Type },
                expression, Expression.Quote(predicate));
        }
    }
}
