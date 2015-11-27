using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for making member access null-safe.
    /// </summary>
    /// <remarks>
    /// Use <see cref="NullsafeQueryBuilder" /> to make a query null-safe.
    /// </remarks>
    public class NullsafeQueryRewriter : ExpressionVisitor
    {
        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null || node.Expression == null)
                return node;

            var fallback = Fallback(node.Type);

            // check both, expression's value and expression's member's value, if not default
            if (fallback.NodeType != ExpressionType.Default)
            {
                return Expression.Condition(
                    Expression.OrElse(
                        Expression.Equal(Visit(node.Expression), Expression.Default(node.Expression.Type)),
                        Expression.Equal(node, Expression.Default(node.Type))),
                    fallback,
                    node);
            }

            // just check expression's value...
            return Expression.Condition(
                Expression.Equal(Visit(node.Expression), Expression.Default(node.Expression.Type)),
                fallback,
                node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null || node.Method == null)
                return node;

            if (node.Object != null)
            {
                var fallback = Fallback(node.Type);

                // check both, object's value and method's result's value, if not default
                if (fallback.NodeType != ExpressionType.Default)
                {
                    return Expression.Condition(
                        Expression.OrElse(
                            Expression.Equal(Visit(node.Object), Expression.Default(node.Object.Type)),
                            Expression.Equal(node, Expression.Default(node.Type))),
                        fallback,
                        node);
                }

                // just check object's value...
                return Expression.Condition(
                    Expression.Equal(Visit(node.Object), Expression.Default(node.Object.Type)),
                    fallback,
                    node);
            }

            return base.VisitMethodCall(node);
        }

        static Expression Fallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                if (typeDefinition == typeof(IEnumerable<>) ||
                    typeDefinition == typeof(ICollection<>))
                {
                    var typeArguments = type.GenericTypeArguments;
                    return Expression.Convert(
                        Expression.New(typeof(List<>).MakeGenericType(typeArguments)),
                        type);
                }
                if (typeDefinition == typeof(ISet<>))
                {
                    var typeArguments = type.GenericTypeArguments;
                    return Expression.Convert(
                        Expression.New(typeof(HashSet<>).MakeGenericType(typeArguments)),
                        type);
                }
            }

            // default value for arrays
            if (type.IsArray)
            {
                return Expression.NewArrayInit(type.GetElementType());
            }

            // default value
            return Expression.Default(type);
        }
    }
}
