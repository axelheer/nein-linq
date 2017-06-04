using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    /// <summary>
    /// Helps building dynamic queries.
    /// </summary>
    public static class DynamicQuery
    {
        static readonly ObjectCache<Type, Func<string, IFormatProvider, object>> cache = new ObjectCache<Type, Func<string, IFormatProvider, object>>();

        /// <summary>
        /// Create a dynamic predicate for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <param name="provider">The culture-specific formatting information.</param>
        /// <returns>The dynamic predicate.</returns>
        public static Expression<Func<T, bool>> CreatePredicate<T>(string selector, DynamicCompare comparer, string value, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
                throw new ArgumentOutOfRangeException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return Expression.Lambda<Func<T, bool>>(CreateComparison(target, selector, comparer, value, provider), target);
        }

        /// <summary>
        /// Create a dynamic predicate for a given property selector, comparison method and reference value.
        /// </summary>
        /// <typeparam name="T">The type of the query data.</typeparam>
        /// <param name="selector">The property selector to parse.</param>
        /// <param name="comparer">The comparison method to use.</param>
        /// <param name="value">The reference value to compare with.</param>
        /// <param name="provider">The culture-specific formatting information.</param>
        /// <returns>The dynamic predicate.</returns>
        public static Expression<Func<T, bool>> CreatePredicate<T>(string selector, string comparer, string value, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(selector))
                throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrEmpty(comparer))
                throw new ArgumentNullException(nameof(comparer));

            var target = Expression.Parameter(typeof(T));

            return Expression.Lambda<Func<T, bool>>(CreateComparison(target, selector, comparer, value, provider), target);
        }

        internal static Expression CreateComparison(ParameterExpression target, string selector, DynamicCompare comparer, string value, IFormatProvider provider)
        {
            var memberAccess = CreateMemberAccess(target, selector);
            var actualValue = CreateConstant(target, memberAccess, value, provider);

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

        internal static Expression CreateComparison(ParameterExpression target, string selector, string comparer, string value, IFormatProvider provider)
        {
            var memberAccess = CreateMemberAccess(target, selector);
            var actualValue = CreateConstant(target, memberAccess, value, provider);

            return Expression.Call(memberAccess, comparer, null, actualValue);
        }

        internal static Expression CreateMemberAccess(Expression target, string selector)
        {
            return selector.Split('.').Aggregate(target, (t, n) => Expression.PropertyOrField(t, n));
        }

        static Expression CreateConstant(ParameterExpression target, Expression selector, string value, IFormatProvider provider)
        {
            var type = Expression.Lambda(selector, target).ReturnType;

            if (string.IsNullOrEmpty(value))
                return Expression.Default(type);

            var converter = cache.GetOrAdd(type, CreateConverter);
            var convertedValue = converter(value, provider);

            return Expression.Constant(convertedValue, type);
        }

        static Func<string, IFormatProvider, object> CreateConverter(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            var target = Expression.Parameter(typeof(string));
            var format = Expression.Parameter(typeof(IFormatProvider));

            var expression = (Expression)target;

            var ordinalParse = underlyingType.GetRuntimeMethod("Parse", new[] { typeof(string) });
            if (ordinalParse != null)
                expression = Expression.Call(ordinalParse, target);

            var cultureParse = underlyingType.GetRuntimeMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });
            if (cultureParse != null)
                expression = Expression.Call(cultureParse, target, format);

            return Expression.Lambda<Func<string, IFormatProvider, object>>(
                Expression.Convert(expression, typeof(object)), target, format).Compile();
        }
    }
}
