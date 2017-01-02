using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#if !NET40

using System.Reflection;

#endif

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for making member access null-safe.
    /// </summary>
    public class NullsafeQueryRewriter : ExpressionVisitor
    {
        static readonly ObjectCache<Type, Expression> cache = new ObjectCache<Type, Expression>();

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Expression != null)
            {
                return MakeNullsafe(node, node.Expression);
            }

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Object != null)
            {
                return MakeNullsafe(node, node.Object);
            }

            return base.VisitMethodCall(node);
        }

        Expression MakeNullsafe(Expression node, Expression value)
        {
            // cache "fallback expression" for performance reasons
            var fallback = cache.GetOrAdd(node.Type, NodeFallback);

            // check value and insert additional coalesce, if fallback is not default
            return Expression.Condition(
                Expression.NotEqual(Visit(value), Expression.Default(value.Type)),
                fallback.NodeType != ExpressionType.Default ? Expression.Coalesce(node, fallback) : node,
                fallback);
        }

        static Expression NodeFallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType() && type.GenericTypeArguments().Length == 1)
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
            var collectionType = collectionDefinition.MakeGenericType(type.GenericTypeArguments());

            // try if an instance of this collection would suffice
            if (type.GetTypeInfo().IsAssignableFrom(collectionType.GetTypeInfo()))
            {
                return Expression.Convert(Expression.New(collectionType), type);
            }

            return null;
        }
    }
}
