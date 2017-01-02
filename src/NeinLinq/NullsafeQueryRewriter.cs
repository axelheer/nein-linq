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

            var result = (MemberExpression)base.VisitMember(node);

            return MakeNullsafe(result, result.Expression);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var result = (MethodCallExpression)base.VisitMethodCall(node);

            return MakeNullsafe(result, result.Object);
        }

        static Expression MakeNullsafe(Expression node, Expression target)
        {
            var fallback = cache.GetOrAdd(node.Type, TypeFallback);

            // coalesce to avoid too much conditions, if possible
            if (!IsNull(node.Type, fallback) && !IsDefault(node.Type, fallback))
            {
                node = Expression.Coalesce(node, fallback);
            }

            if (target != null)
            {
                var targetFallback = cache.GetOrAdd(target.Type, TypeFallback);

                // include condition, if null reference is possible
                if (IsNull(target.Type, targetFallback))
                {
                    node = Expression.Condition(Expression.NotEqual(target, targetFallback), node, fallback);
                }
            }

            return node;
        }

        static Expression TypeFallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType() && type.GenericTypeArguments().Length == 1)
            {
                var listFallback = GenericCollectionFallback(typeof(List<>), type);
                if (listFallback != null)
                    return listFallback;

                var hashSetFallback = GenericCollectionFallback(typeof(HashSet<>), type);
                if (hashSetFallback != null)
                    return hashSetFallback;
            }

            // default value for nullables
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return Expression.Convert(Expression.Default(underlyingType), type);
            }

            // default value for strings
            if (type == typeof(string))
            {
                return Expression.Constant(string.Empty, typeof(string));
            }

            // default value for arrays
            if (type.IsArray)
            {
                return Expression.NewArrayInit(type.GetElementType());
            }

            // default value for references
            if (!type.GetTypeInfo().IsValueType)
            {
                return Expression.Constant(null, type);
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

        static bool IsDefault(Type type, Expression value)
        {
            return value.Type == type &&
                value.NodeType == ExpressionType.Default;
        }

        static bool IsNull(Type type, Expression value)
        {
            return value.Type == type &&
                value.NodeType == ExpressionType.Constant &&
                ((ConstantExpression)value).Value == null;
        }
    }
}
