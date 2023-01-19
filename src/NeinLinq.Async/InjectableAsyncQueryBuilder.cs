namespace NeinLinq;

/// <summary>
/// Replaces method calls with lambda expressions.
/// </summary>
public static class InjectableAsyncQueryBuilder
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable ToAsyncInjectable(this IAsyncQueryable value, params Type[] greenlist)
        => value.AsyncRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable ToAsyncInjectable(this IOrderedAsyncQueryable value, params Type[] greenlist)
        => value.AsyncRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IAsyncQueryable<T> ToAsyncInjectable<T>(this IAsyncQueryable<T> value, params Type[] greenlist)
        => value.AsyncRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedAsyncQueryable<T> ToAsyncInjectable<T>(this IOrderedAsyncQueryable<T> value, params Type[] greenlist)
        => value.AsyncRewrite(new InjectableQueryRewriter(greenlist));
}
