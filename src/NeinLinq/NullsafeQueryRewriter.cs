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
            if (node != null && node.Expression != null)
            {
                return MakeNullsafe(node, node.Expression);
            }

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node != null && node.Object != null)
            {
                return MakeNullsafe(node, node.Object);
            }

            return base.VisitMethodCall(node);
        }

        Expression MakeNullsafe(Expression node, Expression value)
        {
            var fallback = Fallback(node.Type);

            // check value and insert additional coalesce, if fallback is not default
            return Expression.Condition(
                Expression.NotEqual(Visit(value), Expression.Default(value.Type)),
                fallback.NodeType != ExpressionType.Default ? Expression.Coalesce(node, fallback) : node,
                fallback);
        }

        static Expression Fallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType && type.GenericTypeArguments.Length == 1)
            {
                return GenericCollectionFallback(typeof(List<>), type)
                    ?? GenericCollectionFallback(typeof(HashSet<>), type)
                    ?? Expression.Default(type);
            }

            // default value for arrays
            if (type.IsArray)
            {
                return Expression.NewArrayInit(type.GetElementType());
            }

            // default value
            return Expression.Default(type);
        }

        static Expression GenericCollectionFallback(Type collectionDefinition, Type type)
        {
            var collectionType = collectionDefinition.MakeGenericType(type.GenericTypeArguments);

            if (type.GetTypeInfo().IsAssignableFrom(collectionType.GetTypeInfo()))
            {
                return Expression.Convert(Expression.New(collectionType), type);
            }

            return null;
        }
    }
}
