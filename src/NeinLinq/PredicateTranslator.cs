namespace NeinLinq;

/// <summary>
/// Provides methods for combining and translating lambda expressions representing predicates.
/// </summary>
public static class PredicateTranslator
{
    /// <summary>
    /// Starts translation of a given predicate.
    /// </summary>
    /// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
    /// <param name="predicate">The predicate expression to translate.</param>
    /// <returns>A translation object for the given predicate.</returns>
    public static PredicateTranslation<TSource> Translate<TSource>(this Expression<Func<TSource, bool>> predicate)
        => new(predicate);

    /// <summary>
    /// Combines two given predicates using a conditional AND operation.
    /// </summary>
    /// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
    /// <param name="left">The first predicate expression to combine.</param>
    /// <param name="right">The second predicate expression to combine.</param>
    /// <returns>A single combined predicate expression.</returns>
    public static Expression<Func<TSource, bool>> And<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        var l = left.Parameters[0];
        var r = right.Parameters[0];

        var binder = new ParameterBinder(l, r);

        return Expression.Lambda<Func<TSource, bool>>(
            Expression.AndAlso(binder.Visit(left.Body), right.Body), r);
    }

    /// <summary>
    /// Combines two given predicates using a conditional OR operation.
    /// </summary>
    /// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
    /// <param name="left">The first predicate expression to combine.</param>
    /// <param name="right">The second predicate expression to combine.</param>
    /// <returns>A single combined predicate expression.</returns>
    public static Expression<Func<TSource, bool>> Or<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));

        var l = left.Parameters[0];
        var r = right.Parameters[0];

        var binder = new ParameterBinder(l, r);

        return Expression.Lambda<Func<TSource, bool>>(
            Expression.OrElse(binder.Visit(left.Body), right.Body), r);
    }

    /// <summary>
    /// Negates the given predicate using a binary NOT operation.
    /// </summary>
    /// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
    /// <param name="predicate">The predicate expression.</param>
    /// <returns>A predicate expression.</returns>
    public static Expression<Func<TSource, bool>> Not<TSource>(this Expression<Func<TSource, bool>> predicate)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        return Expression.Lambda<Func<TSource, bool>>(
            Expression.Not(predicate.Body), predicate.Parameters);
    }
}
