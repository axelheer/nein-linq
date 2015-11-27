using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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

            // check value and insert additional coalesce, if fallback is not default
            return Expression.Condition(
                Expression.NotEqual(Visit(node.Expression), Expression.Default(node.Expression.Type)),
                fallback.NodeType != ExpressionType.Default ? (Expression)Expression.Coalesce(node, fallback) : node,
                fallback);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null || node.Method == null)
                return node;

            // only non static method calls can trigger null reference...
            if (node.Object != null)
            {
                var fallback = Fallback(node.Type);

                // check result and insert additional coalesce, if fallback is not default
                return Expression.Condition(
                    Expression.NotEqual(Visit(node.Object), Expression.Default(node.Object.Type)),
                    fallback.NodeType != ExpressionType.Default ? (Expression)Expression.Coalesce(node, fallback) : node,
                    fallback);
            }

            return base.VisitMethodCall(node);
        }

        static Expression Fallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType && type.GenericTypeArguments.Length == 1)
            {
                var typeDefinition = type.GetGenericTypeDefinition();

                var listType = typeof(List<>).MakeGenericType(type.GenericTypeArguments);
                if (type.GetTypeInfo().IsAssignableFrom(listType.GetTypeInfo()))
                {
                    return Expression.Convert(Expression.New(listType), type);
                }

                var hashSetType = typeof(HashSet<>).MakeGenericType(type.GenericTypeArguments);
                if (type.GetTypeInfo().IsAssignableFrom(hashSetType.GetTypeInfo()))
                {
                    return Expression.Convert(Expression.New(hashSetType), type);
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
