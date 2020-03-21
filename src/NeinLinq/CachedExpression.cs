using System;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// An expression. Cached. Surprise.
    /// </summary>
    public sealed class CachedExpression<TDelegate>
    {
        private readonly Expression<TDelegate> expression;
        private readonly Lazy<TDelegate> lazyCompiled;

        /// <summary>
        /// Create a new cached expression.
        /// </summary>
        /// <param name="expression">Expression to cache.</param>
        public CachedExpression(Expression<TDelegate> expression)
        {
            this.expression = expression ?? throw new ArgumentNullException(nameof(expression));

            lazyCompiled = new Lazy<TDelegate>(this.expression.Compile);
        }

        /// <summary>
        /// The actual expression.
        /// </summary>
        public Expression<TDelegate> Expression => expression;

        /// <summary>
        /// The compiled expression.
        /// </summary>
        public TDelegate Compiled => lazyCompiled.Value;

        /// <summary>
        /// Returns the original expression.
        /// </summary>
        /// <returns>The original expression.</returns>
        public LambdaExpression ToLambdaExpression() => expression;

        /// <summary>
        /// Returns the original expression.
        /// </summary>
        /// <param name="cached">The cached expression.</param>
        /// <returns>The original expression.</returns>
        public static implicit operator LambdaExpression(CachedExpression<TDelegate> cached) => cached?.expression!;
    }

    /// <summary>
    /// Helper for cached expressions.
    /// </summary>
    public abstract class CachedExpression
    {
        /// <summary>
        /// Create a new cached expression.
        /// </summary>
        /// <param name="expression">Expression to cache.</param>
        /// <returns>The cached expression.</returns>
        public static CachedExpression<TDelegate> From<TDelegate>(Expression<TDelegate> expression) => new CachedExpression<TDelegate>(expression);
    }
}
