using System;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for making member access null-safe.
    /// </summary>
    public class NullsafeQueryRewriter : ExpressionVisitor
    {
        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var result = (MemberExpression)base.VisitMember(node);

            if (!IsSafe(result.Expression))
            {
                // insert null-check before accessing property or field
                return BeSafe(result, result.Expression);
            }

            return result;
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var result = (MethodCallExpression)base.VisitMethodCall(node);

            if (!IsSafe(result.Object))
            {
                // insert null-check before invoking instance method
                return BeSafe(result, result.Object);
            }

            if (result.Method.IsExtensionMethod() && !IsSafe(result.Arguments[0]))
            {
                // insert null-check before invoking extension method
                return BeSafe(result, result.Arguments[0]);
            }

            return result;
        }

        static Expression BeSafe(Expression expression, Expression target)
        {
            // target can be null, which is why we are actually here...
            var targetFallback = Expression.Constant(null, target.Type);

            // expression can be default or null, which is basically the same...
            var expressionFallback = !expression.Type.IsNullableOrReferenceType()
                ? (Expression)Expression.Default(expression.Type) : Expression.Constant(null, expression.Type);

            return Expression.Condition(Expression.Equal(target, targetFallback), expressionFallback, expression);
        }

        static bool IsSafe(Expression expression)
        {
            // in method call results and constant values we trust to avoid too much conditions...
            return expression == null
                || expression.NodeType == ExpressionType.Call
                || expression.NodeType == ExpressionType.Constant
                || !expression.Type.IsNullableOrReferenceType();
        }
    }
}
