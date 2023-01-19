namespace NeinLinq;

/// <summary>
/// Replaces method calls with lambda expressions.
/// </summary>
public static class InjectableEntityQueryBuilder
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable ToEntityInjectable(this IQueryable value, params Type[] greenlist)
        => value.EntityRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable ToEntityInjectable(this IOrderedQueryable value, params Type[] greenlist)
        => value.EntityRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IQueryable<T> ToEntityInjectable<T>(this IQueryable<T> value, params Type[] greenlist)
        => value.EntityRewrite(new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <typeparam name="T">The type of the query data.</typeparam>
    /// <param name="value">A query.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>A query proxy.</returns>
    public static IOrderedQueryable<T> ToEntityInjectable<T>(this IOrderedQueryable<T> value, params Type[] greenlist)
        => value.EntityRewrite(new InjectableQueryRewriter(greenlist));
}
