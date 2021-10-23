namespace NeinLinq;

/// <summary>
/// An expression. Cached. Surprise.
/// </summary>
public sealed class CachedExpression<TDelegate>
{
    /// <summary>
    /// The actual expression.
    /// </summary>
    public Expression<TDelegate> Expression { get; }

    private readonly Lazy<TDelegate> lazyCompiled;

    /// <summary>
    /// The compiled expression.
    /// </summary>
    public TDelegate Compiled
        => lazyCompiled.Value;

    /// <summary>
    /// Create a new cached expression.
    /// </summary>
    /// <param name="expression">Expression to cache.</param>
    public CachedExpression(Expression<TDelegate> expression)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));

        lazyCompiled = new Lazy<TDelegate>(expression.Compile);
    }

#pragma warning disable CA2225

    /// <summary>
    /// Create a new cached expression.
    /// </summary>
    /// <param name="expression">Expression to cache.</param>
    /// <returns>The cached expression.</returns>
    public static implicit operator CachedExpression<TDelegate>(Expression<TDelegate> expression)
        => new(expression);

    /// <summary>
    /// Returns the original expression.
    /// </summary>
    /// <param name="cached">The cached expression.</param>
    /// <returns>The original expression.</returns>
    public static implicit operator Expression<TDelegate>(CachedExpression<TDelegate> cached)
        => cached?.Expression!;

    /// <summary>
    /// Returns the original expression. Untyped.
    /// </summary>
    /// <param name="cached">The cached expression.</param>
    /// <returns>The original expression.</returns>
    public static implicit operator LambdaExpression(CachedExpression<TDelegate> cached)
        => cached?.Expression!;

#pragma warning restore CA2225

}

/// <summary>
/// Helper for cached expressions.
/// </summary>
public static class CachedExpression
{
    /// <summary>
    /// Create a new cached expression.
    /// </summary>
    /// <param name="expression">Expression to cache.</param>
    /// <returns>The cached expression.</returns>
    public static CachedExpression<TDelegate> From<TDelegate>(Expression<TDelegate> expression)
        => expression;
}
