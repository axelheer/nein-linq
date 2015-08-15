using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Helps building dynamic queries.
    /// </summary>
    public static class DynamicQuery
    {
        /// <summary>
        /// Create a dynamic predicate for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The dynamic predicate.</returns>
        public static Expression<Func<T, bool>> CreatePredicate<T>(string selector, DynamicCompare comparer, string value)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
                throw new ArgumentOutOfRangeException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return Expression.Lambda<Func<T, bool>>(CreateComparison(target, selector, comparer, value), target);
        }

        /// <summary>
        /// Create a dynamic predicate for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <returns>The dynamic predicate.</returns>
        public static Expression<Func<T, bool>> CreatePredicate<T>(string selector, string comparer, string value)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrEmpty(comparer))
                throw new ArgumentNullException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return Expression.Lambda<Func<T, bool>>(CreateComparison(target, selector, comparer, value), target);
        }

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

            return CreateFilteredQuery(query, selector, comparer, value);
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

            return (IQueryable<T>)CreateFilteredQuery(query, selector, comparer, value);
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

            return CreateFilteredQuery(query, selector, comparer, value);
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

            return (IQueryable<T>)CreateFilteredQuery(query, selector, comparer, value);
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

            var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            return (IOrderedQueryable)CreateSortedQuery(query, selector, method);
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

            var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            return (IOrderedQueryable<T>)CreateSortedQuery(query, selector, method);
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

            var method = descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

            return (IOrderedQueryable)CreateSortedQuery(query, selector, method);
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

            var method = descending ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);

            return (IOrderedQueryable<T>)CreateSortedQuery(query, selector, method);
        }

        private static IQueryable CreateSortedQuery(IQueryable query, string selector, string method)
        {
            var target = Expression.Parameter(query.ElementType);
            var keySelector = Expression.Lambda(CreateMemberAccess(target, selector), target);

            var expression = Expression.Call(typeof(Queryable), method, new[] { target.Type, keySelector.ReturnType },
                query.Expression, Expression.Quote(keySelector));

            return query.Provider.CreateQuery(expression);
        }

        private static IQueryable CreateFilteredQuery(IQueryable query, string selector, DynamicCompare comparer, string value)
        {
            var target = Expression.Parameter(query.ElementType);
            var predicate = Expression.Lambda(CreateComparison(target, selector, comparer, value), target);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { target.Type },
                query.Expression, Expression.Quote(predicate));

            return query.Provider.CreateQuery(expression);
        }

        private static IQueryable CreateFilteredQuery(IQueryable query, string selector, string comparer, string value)
        {
            var target = Expression.Parameter(query.ElementType);
            var predicate = Expression.Lambda(CreateComparison(target, selector, comparer, value), target);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { target.Type },
                query.Expression, Expression.Quote(predicate));

            return query.Provider.CreateQuery(expression);
        }

        private static Expression CreateComparison(Expression target, string selector, DynamicCompare comparer, string value)
        {
            var memberAccess = CreateMemberAccess(target, selector);
            var actualValue = CreateConstant(target, memberAccess, value);

            switch (comparer)
            {
                case DynamicCompare.Equal:
                    return Expression.Equal(memberAccess, actualValue);
                case DynamicCompare.NotEqual:
                    return Expression.NotEqual(memberAccess, actualValue);
                case DynamicCompare.GreaterThan:
                    return Expression.GreaterThan(memberAccess, actualValue);
                case DynamicCompare.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(memberAccess, actualValue);
                case DynamicCompare.LessThan:
                    return Expression.LessThan(memberAccess, actualValue);
                case DynamicCompare.LessThanOrEqual:
                    return Expression.LessThanOrEqual(memberAccess, actualValue);
            }

            throw new InvalidOperationException();
        }

        private static Expression CreateComparison(Expression target, string selector, string comparer, string value)
        {
            var memberAccess = CreateMemberAccess(target, selector);
            var actualValue = CreateConstant(target, memberAccess, value);

            return Expression.Call(memberAccess, comparer, null, actualValue);
        }

        private static Expression CreateMemberAccess(Expression target, string selector)
        {
            return selector.Split('.').Aggregate(target, (t, n) => Expression.PropertyOrField(t, n));
        }

        private static Expression CreateConstant(Expression target, Expression selector, string value)
        {
            var type = Expression.Lambda(selector, (ParameterExpression)target).ReturnType;
            var conversionType = Nullable.GetUnderlyingType(type) ?? type;

            if (type != conversionType && string.IsNullOrEmpty(value))
                return Expression.Default(type);

            return Expression.Constant(Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture), type);
        }
    }
}
