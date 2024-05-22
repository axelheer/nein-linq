namespace NeinLinq;

/// <summary>
/// Helps building dynamic expressions.
/// </summary>
public static class DynamicExpression
{
    private static readonly ObjectCache<Type, Func<string, IFormatProvider?, object>> Cache
        = new();

    /// <summary>
    /// Create a dynamic comparison expression for a given property selector, comparison method and reference value.
    /// </summary>
    /// <param name="target">The parameter of the query data.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The dynamic comparison expression.</returns>
    public static Expression CreateComparison(ParameterExpression target,
                                              string selector,
                                              DynamicCompare comparer,
                                              string? value,
                                              IFormatProvider? provider = null)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (!Enum.IsDefined(typeof(DynamicCompare), comparer))
            throw new ArgumentOutOfRangeException(nameof(comparer));

        var memberAccess = CreateMemberAccess(target, selector);
        var actualValue = CreateConstant(target, memberAccess, value, provider);

        return comparer switch
        {
            DynamicCompare.Equal => Expression.Equal(memberAccess, actualValue),
            DynamicCompare.NotEqual => Expression.NotEqual(memberAccess, actualValue),
            DynamicCompare.GreaterThan => Expression.GreaterThan(memberAccess, actualValue),
            DynamicCompare.GreaterThanOrEqual => Expression.GreaterThanOrEqual(memberAccess, actualValue),
            DynamicCompare.LessThan => Expression.LessThan(memberAccess, actualValue),
            DynamicCompare.LessThanOrEqual => Expression.LessThanOrEqual(memberAccess, actualValue),
            _ => Expression.Constant(false),
        };
    }

    /// <summary>
    /// Create a dynamic comparison expression for a given property selector, comparison method and reference value.
    /// </summary>
    /// <param name="target">The parameter of the query data.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <param name="comparer">The comparison method to use.</param>
    /// <param name="value">The reference value to compare with.</param>
    /// <param name="provider">The culture-specific formatting information.</param>
    /// <returns>The dynamic comparison expression.</returns>
    public static Expression CreateComparison(ParameterExpression target,
                                              string selector,
                                              string comparer,
                                              string? value,
                                              IFormatProvider? provider = null)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));
        if (string.IsNullOrEmpty(comparer))
            throw new ArgumentNullException(nameof(comparer));

        var memberAccess = CreateMemberAccess(target, selector);
        var actualValue = CreateConstant(target, memberAccess, value, provider);

        return Expression.Call(memberAccess, comparer, null, actualValue);
    }

    /// <summary>
    /// Creates a dynamic member access expression.
    /// </summary>
    /// <param name="target">The parameter of the query data.</param>
    /// <param name="selector">The property selector to parse.</param>
    /// <returns>The dynamic member access expression.</returns>
    public static Expression CreateMemberAccess(Expression target,
                                                string selector)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));
        if (string.IsNullOrEmpty(selector))
            throw new ArgumentNullException(nameof(selector));

        return selector.Split('.').Aggregate(target, Expression.PropertyOrField);
    }

    /// <summary>
    /// Registers a custom converter for any type.
    /// </summary>
    /// <param name="type">Tye type to convert.</param>
    /// <param name="converter">The culture-specific converter.</param>
    public static void RegisterConverter(Type type, Func<string, IFormatProvider?, object> converter)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));
        if (converter is null)
            throw new ArgumentNullException(nameof(converter));

        Cache.Add(type, converter);
    }

    private static Expression CreateConstant(ParameterExpression target,
                                             Expression selector,
                                             string? value,
                                             IFormatProvider? provider)
    {
        var type = Expression.Lambda(selector, target).ReturnType;

        if (string.IsNullOrEmpty(value))
            return Expression.Default(type);

        var converter = Cache.GetOrAdd(type, CreateConverter);
        var convertedValue = converter(value!, provider);

        return Expression.Constant(convertedValue, type);
    }

    private static Func<string, IFormatProvider?, object> CreateConverter(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (underlyingType.IsEnum)
            return (value, _) => Enum.Parse(underlyingType, value);

        var target = Expression.Parameter(typeof(string));
        var format = Expression.Parameter(typeof(IFormatProvider));

        var expression = (Expression)target;

        var ordinalParse = underlyingType.GetMethod("Parse", [typeof(string)]);
        if (ordinalParse is not null)
            expression = Expression.Call(ordinalParse, target);

        var cultureParse = underlyingType.GetMethod("Parse", [typeof(string), typeof(IFormatProvider)]);
        if (cultureParse is not null)
            expression = Expression.Call(cultureParse, target, format);

        return Expression.Lambda<Func<string?, IFormatProvider?, object>>(
            Expression.Convert(expression, typeof(object)), target, format).Compile();
    }
}
