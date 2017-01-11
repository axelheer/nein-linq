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

            var target = Visit(node.Expression);

            if (!IsSafe(target))
            {
                // insert null-check before accessing property or field
                return BeSafe(node, target, node.Update);
            }

            return node.Update(target);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var target = Visit(node.Object);

            if (!IsSafe(target))
            {
                // insert null-check before invoking instance method
                return BeSafe(node, target, fallback => node.Update(fallback, node.Arguments));
            }

            var arguments = Visit(node.Arguments);

            if (node.Method.IsExtensionMethod() && !IsSafe(arguments[0]))
            {
                // insert null-check before invoking extension method
                return BeSafe(node, arguments[0], fallback =>
                {
                    var args = new Expression[arguments.Count];
                    arguments.CopyTo(args, 0);
                    args[0] = fallback;

                    return node.Update(null, args);
                });
            }

            return node.Update(target, arguments);
        }

        static Expression BeSafe(Expression expression, Expression target, Func<Expression, Expression> update)
        {
            var fallback = cache.GetOrAdd(target.Type, Fallback);

            if (fallback != null)
            {
                // coalesce instead, a bit intrusive but fast...
                return update(Expression.Coalesce(target, fallback));
            }

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

        static Expression Fallback(Type type)
        {
            // default values for generic collections
            if (type.IsConstructedGenericType() && type.GenericTypeArguments().Length == 1)
            {
                return CollectionFallback(typeof(List<>), type)
                    ?? CollectionFallback(typeof(HashSet<>), type);
            }

            // default value for arrays
            if (type.IsArray)
            {
                return Expression.NewArrayInit(type.GetElementType());
            }

            return null;
        }

        static Expression CollectionFallback(Type definition, Type type)
        {
            var collection = definition.MakeGenericType(type.GenericTypeArguments());

            // try if an instance of this collection would suffice
            if (type.GetTypeInfo().IsAssignableFrom(collection.GetTypeInfo()))
            {
                return Expression.Convert(Expression.New(collection), type);
            }

            return null;
        }
    }
}
