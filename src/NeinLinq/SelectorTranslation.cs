namespace NeinLinq;

/// <summary>
/// Represents a translation of a given selector.
/// </summary>
/// <typeparam name="TSource">The type of the selector's source parameter.</typeparam>
/// <typeparam name="TResult">The type of the selector's result parameter.</typeparam>
public class SelectorTranslation<TSource, TResult>
{
    private readonly Expression<Func<TSource, TResult>> selector;

    /// <summary>
    /// Starts translation of a given selector.
    /// </summary>
    /// <param name="selector">The selector expression to translate.</param>
    public SelectorTranslation(Expression<Func<TSource, TResult>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        this.selector = selector;
    }

    /// <summary>
    /// Translates a given selector for a given subtype using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TTranslatedSource, TResult>> Source<TTranslatedSource>()
        where TTranslatedSource : TSource
    {
        var s = selector.Parameters[0];
        var t = Expression.Parameter(typeof(TTranslatedSource), s.Name);

        var binder = new ParameterBinder(s, t);

        return Expression.Lambda<Func<TTranslatedSource, TResult>>(
            binder.Visit(selector.Body), t);
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="path">The path from the desired type to the given type.</param>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TTranslatedSource, TResult>> Source<TTranslatedSource>(Expression<Func<TTranslatedSource, TSource>> path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        var s = selector.Parameters[0];
        var t = path.Parameters[0];

        var binder = new ParameterBinder(s, path.Body);

        return Expression.Lambda<Func<TTranslatedSource, TResult>>(
            binder.Visit(selector.Body), t);
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TTranslatedSource, TResult>> Source<TTranslatedSource>(Expression<Func<TTranslatedSource, Func<TSource, TResult>, TResult>> translation)
    {
        if (translation is null)
            throw new ArgumentNullException(nameof(translation));

        var t = translation.Parameters[0];
        var s = translation.Parameters[1];

        var binder = new ParameterBinder(s, selector);

        return Expression.Lambda<Func<TTranslatedSource, TResult>>(
            binder.Visit(translation.Body), t);
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TTranslatedSource, IEnumerable<TResult>>> Source<TTranslatedSource>(Expression<Func<TTranslatedSource, Func<TSource, TResult>, IEnumerable<TResult>>> translation)
    {
        if (translation is null)
            throw new ArgumentNullException(nameof(translation));

        var t = translation.Parameters[0];
        var s = translation.Parameters[1];

        var binder = new ParameterBinder(s, selector);

        return Expression.Lambda<Func<TTranslatedSource, IEnumerable<TResult>>>(
            binder.Visit(translation.Body), t);
    }

    /// <summary>
    /// Translates a given selector for a given subtype using it's result parameter.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Result<TTranslatedResult>()
        where TTranslatedResult : TResult
    {
        if (selector.Body is MemberInitExpression init)
        {
            if (init.NewExpression.Arguments.Count > 0)
                throw new NotSupportedException("Only parameterless constructors are supported yet.");

            var s = selector.Parameters[0];

            return Expression.Lambda<Func<TSource, TTranslatedResult>>(
                Expression.MemberInit(Expression.New(typeof(TTranslatedResult)), init.Bindings), s);
        }

        throw new NotSupportedException("Only member init expressions are supported yet.");
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's result parameter.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="path">The path from the desired type to the given type.</param>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Result<TTranslatedResult>(Expression<Func<TTranslatedResult, TResult>> path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        if (path.Body is MemberExpression member)
        {
            var s = selector.Parameters[0];

            var bind = Expression.Bind(member.Member, selector.Body);

            return Expression.Lambda<Func<TSource, TTranslatedResult>>(
                Expression.MemberInit(Expression.New(typeof(TTranslatedResult)), bind), s);
        }

        throw new NotSupportedException("Only member expressions are supported yet.");
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's result parameter.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <returns>A translated selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Result<TTranslatedResult>(Expression<Func<TSource, Func<TSource, TResult>, TTranslatedResult>> translation)
    {
        if (translation is null)
            throw new ArgumentNullException(nameof(translation));

        var s = translation.Parameters[0];
        var t = translation.Parameters[1];

        var binder = new ParameterBinder(t, selector);

        return Expression.Lambda<Func<TSource, TTranslatedResult>>(
            binder.Visit(translation.Body), s);
    }

    /// <summary>
    /// Continues translation of a given selector for a given subtype using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <returns>Another translation object for the given selector.</returns>
    public SelectorTranslation<TTranslatedSource, TResult> Cross<TTranslatedSource>()
        where TTranslatedSource : TSource
        => Source<TTranslatedSource>().Translate();

    /// <summary>
    /// Continues translation of a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="path">The path from the desired type to the given type.</param>
    /// <returns>Another translation object for the given selector.</returns>
    public SelectorTranslation<TTranslatedSource, TResult> Cross<TTranslatedSource>(Expression<Func<TTranslatedSource, TSource>> path)
        => Source(path).Translate();

    /// <summary>
    /// Continues translation of a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <returns>Another translation object for the given selector.</returns>
    public SelectorTranslation<TTranslatedSource, TResult> Cross<TTranslatedSource>(Expression<Func<TTranslatedSource, Func<TSource, TResult>, TResult>> translation)
        => Source(translation).Translate();

    /// <summary>
    /// Continues translation of a given selector for a given related type using it's source parameter.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <returns>Another translation object for the given selector.</returns>
    public SelectorTranslation<TTranslatedSource, IEnumerable<TResult>> Cross<TTranslatedSource>(Expression<Func<TTranslatedSource, Func<TSource, TResult>, IEnumerable<TResult>>> translation)
        => Source(translation).Translate();

    /// <summary>
    /// Translates a given selector for a given subtype using it's result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Apply<TTranslatedResult>(Expression<Func<TSource, TTranslatedResult>> value)
        where TTranslatedResult : TResult
        => Result<TTranslatedResult>().Apply(value);

    /// <summary>
    /// Translates a given selector for a given related type using it's result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="path">The path from the desired type to the given type.</param>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Apply<TTranslatedResult>(Expression<Func<TTranslatedResult, TResult>> path, Expression<Func<TSource, TTranslatedResult>> value)
        => Result(path).Apply(value);

    /// <summary>
    /// Translates a given selector for a given related type using it's result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TSource, TTranslatedResult>> Apply<TTranslatedResult>(Expression<Func<TSource, Func<TSource, TResult>, TTranslatedResult>> translation, Expression<Func<TSource, TTranslatedResult>> value)
        => Result(translation).Apply(value);

    /// <summary>
    /// Translates a given selector for given subtypes using it's source and result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TTranslatedSource, TTranslatedResult>> To<TTranslatedSource, TTranslatedResult>(Expression<Func<TTranslatedSource, TTranslatedResult>>? value = null)
        where TTranslatedSource : TSource
        where TTranslatedResult : TResult
    {
        var result = Cross<TTranslatedSource>().Result<TTranslatedResult>();

        return value is not null ? result.Apply(value) : result;
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's source and result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="sourcePath">The path from the desired source type to the given type.</param>
    /// <param name="resultPath">The path from the desired result type to the given type.</param>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TTranslatedSource, TTranslatedResult>> To<TTranslatedSource, TTranslatedResult>(Expression<Func<TTranslatedSource, TSource>> sourcePath, Expression<Func<TTranslatedResult, TResult>> resultPath, Expression<Func<TTranslatedSource, TTranslatedResult>>? value = null)
    {
        var result = Cross(sourcePath).Result(resultPath);

        return value is not null ? result.Apply(value) : result;
    }

    /// <summary>
    /// Translates a given selector for a given related type using it's source and result parameter
    /// and combines it with another given selector by merging their member bindings.
    /// </summary>
    /// <typeparam name="TTranslatedSource">The type of the translated selector's source parameter.</typeparam>
    /// <typeparam name="TTranslatedResult">The type of the translated selector's result parameter.</typeparam>
    /// <param name="translation">The translation from the desired type to the given type,
    /// using the initially given selector to be injected into a new selector.</param>
    /// <param name="value">The additional selector expression to combine.</param>
    /// <returns>A single translated and combined selector expression.</returns>
    public Expression<Func<TTranslatedSource, TTranslatedResult>> To<TTranslatedSource, TTranslatedResult>(Expression<Func<TTranslatedSource, Func<TSource, TResult>, TTranslatedResult>> translation, Expression<Func<TTranslatedSource, TTranslatedResult>>? value = null)
    {
        if (translation is null)
            throw new ArgumentNullException(nameof(translation));

        var s = translation.Parameters[0];
        var t = translation.Parameters[1];

        var binder = new ParameterBinder(t, selector);

        var result = Expression.Lambda<Func<TTranslatedSource, TTranslatedResult>>(
            binder.Visit(translation.Body), s);

        return value is not null ? result.Apply(value) : result;
    }
}
