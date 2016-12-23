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

        internal static Expression CreateComparison(ParameterExpression target, string selector, DynamicCompare comparer, string value)
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

                default:
                    return Expression.Constant(false);
            }
        }

        internal static Expression CreateComparison(ParameterExpression target, string selector, string comparer, string value)
        {
            var memberAccess = CreateMemberAccess(target, selector);
            var actualValue = CreateConstant(target, memberAccess, value);

            return Expression.Call(memberAccess, comparer, null, actualValue);
        }

        internal static Expression CreateMemberAccess(Expression target, string selector)
        {
            return selector.Split('.').Aggregate(target, (t, n) => Expression.PropertyOrField(t, n));
        }

        internal static Expression CreateConstant(ParameterExpression target, Expression selector, string value)
        {
            var type = Expression.Lambda(selector, target).ReturnType;

            if (string.IsNullOrEmpty(value))
                return Expression.Default(type);

            var conversionType = Nullable.GetUnderlyingType(type) ?? type;
            var convertedValue = conversionType == typeof(Guid) ? Guid.Parse(value) :
                Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);

            return Expression.Constant(convertedValue, type);
        }
    }
}
