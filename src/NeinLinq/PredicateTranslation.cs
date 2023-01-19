namespace NeinLinq;

/// <summary>
/// Represents a translation of a given predicate.
/// </summary>
/// <typeparam name="TSource">The type of the predicate's parameter.</typeparam>
public class PredicateTranslation<TSource>
{
    private readonly Expression<Func<TSource, bool>> predicate;

    /// <summary>
    /// Creates a new predicate translation.
    /// </summary>
    /// <param name="predicate">The predicate to translate.</param>
    public PredicateTranslation(Expression<Func<TSource, bool>> predicate)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));

        this.predicate = predicate;
    }

    /// <summary>
    /// Translates a given predicate for a given subtype.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated predicate's parameter.</typeparam>
    /// <returns>A translated predicate expression.</returns>
    public Expression<Func<TTranslatedSource, bool>> To<TTranslatedSource>()
        where TTranslatedSource : TSource
    {
        var s = predicate.Parameters[0];
        var t = Expression.Parameter(typeof(TTranslatedSource), s.Name);

        var binder = new ParameterBinder(s, t);

        return Expression.Lambda<Func<TTranslatedSource, bool>>(
            binder.Visit(predicate.Body), t);
    }

    /// <summary>
    /// Translates a given predicate for a given related type.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated predicate's parameter.</typeparam>
    /// <param name="path">The path from the desired type to the given type.</param>
    /// <returns>A translated predicate expression.</returns>
    public Expression<Func<TTranslatedSource, bool>> To<TTranslatedSource>(Expression<Func<TTranslatedSource, TSource>> path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        var s = predicate.Parameters[0];
        var t = path.Parameters[0];

        var binder = new ParameterBinder(s, path.Body);

        return Expression.Lambda<Func<TTranslatedSource, bool>>(
            binder.Visit(predicate.Body), t);
    }

    /// <summary>
    /// Translates a given predicate for a given related type.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated predicate's parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given predicate to be injected into a new predicate.</param>
    /// <returns>A translated predicate expression.</returns>
    public Expression<Func<TTranslatedSource, bool>> To<TTranslatedSource>(Expression<Func<TTranslatedSource, Func<TSource, bool>, bool>> translation)
    {
        if (translation is null)
            throw new ArgumentNullException(nameof(translation));

        var t = translation.Parameters[0];
        var s = translation.Parameters[1];

        var binder = new ParameterBinder(s, predicate);

        return Expression.Lambda<Func<TTranslatedSource, bool>>(
            binder.Visit(translation.Body), t);
    }
}
