namespace NeinLinq;

/// <summary>
/// Provides methods for combining and translating lambda expressions representing selectors.
/// </summary>
public static class SelectorTranslator
{
    /// <summary>
    /// Starts translation of a given selector.
    /// </summary>
    /// <typeparam name="TSource">The type of the selector's source parameter.</typeparam>
    /// <typeparam name="TResult">The type of the selector's result parameter.</typeparam>
    /// <param name="selector">The selector expression to translate.</param>
    /// <returns>A translation object for the given selector.</returns>
    public static SelectorTranslation<TSource, TResult> Translate<TSource, TResult>(this Expression<Func<TSource, TResult>> selector)
        => new(selector);

    /// <summary>
    /// Combines two given selectors by merging their member bindings.
    /// </summary>
    /// <typeparam name="TSource">The type of the selector's source parameter.</typeparam>
    /// <typeparam name="TResult">The type of the selector's result parameter.</typeparam>
    /// <param name="left">The first selector expression to combine.</param>
    /// <param name="right">The second selector expression to combine.</param>
    /// <returns>A single combined selector expression.</returns>
    public static Expression<Func<TSource, TResult>> Apply<TSource, TResult>(this Expression<Func<TSource, TResult>> left, Expression<Func<TSource, TResult>> right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        var leftInit = left.Body as MemberInitExpression;
        var rightInit = right.Body as MemberInitExpression;

        var leftNew = left.Body as NewExpression ?? leftInit?.NewExpression;
        var rightNew = right.Body as NewExpression ?? rightInit?.NewExpression;

        if (leftNew is null || rightNew is null)
            throw new NotSupportedException("Only member init expressions and new expressions are supported yet.");
        if (leftNew.Arguments.Count > 0 || rightNew.Arguments.Count > 0)
            throw new NotSupportedException("Only parameterless constructors are supported yet.");

        var leftBindings = leftInit?.Bindings ?? Enumerable.Empty<MemberBinding>();
        var rightBindings = rightInit?.Bindings ?? Enumerable.Empty<MemberBinding>();

        var l = left.Parameters[0];
        var r = right.Parameters[0];

        var binder = new ParameterBinder(l, r);

        return Expression.Lambda<Func<TSource, TResult>>(
            binder.Visit(Expression.MemberInit(Expression.New(typeof(TResult)), leftBindings.Concat(rightBindings))), r);
    }
}
