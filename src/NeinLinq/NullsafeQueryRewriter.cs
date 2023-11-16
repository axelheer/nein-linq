using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeinLinq;

/// <summary>
/// Expression visitor for making member access null-safe.
/// </summary>
public class NullsafeQueryRewriter : ExpressionVisitor
{
    private static readonly ObjectCache<Type, Expression?> Cache
        = new();

    /// <inheritdoc />
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        var target = Visit(node.Expression);

        if (!IsSafe(target))
        {
            // insert null-check before accessing property or field
            return BeSafe(target!, node, node.Update);
        }

        return node.Update(target!);
    }

    /// <inheritdoc />
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        var target = Visit(node.Object);

        if (!IsSafe(target))
        {
            // insert null-check before invoking instance method
            return BeSafe(target!, node, fallback => node.Update(fallback, node.Arguments));
        }

        var arguments = Visit(node.Arguments);

        if (IsExtensionMethod(node.Method) && !IsSafe(arguments[0]))
        {
            // insert null-check before invoking extension method
            return BeSafe(arguments[0], node.Update(target!, arguments), fallback =>
            {
                var args = new Expression[arguments.Count];
                arguments.CopyTo(args, 0);
                args[0] = fallback;

                return node.Update(target!, args);
            });
        }

        return node.Update(target!, arguments);
    }

    private static Expression BeSafe(Expression target, Expression expression, Func<Expression, Expression> update)
    {
        var fallback = Cache.GetOrAdd(target.Type, Fallback);

        if (fallback is not null)
        {
            // coalesce instead, a bit intrusive but fast...
            return update(Expression.Coalesce(target, fallback));
        }

        // target can be null, which is why we are actually here...
        var targetFallback = Expression.Constant(null, target.Type);

        // expression can be default or null, which is basically the same...
        var expressionFallback = !IsNullableOrReferenceType(expression.Type)
            ? (Expression)Expression.Default(expression.Type) : Expression.Constant(null, expression.Type);

        return Expression.Condition(Expression.Equal(target, targetFallback), expressionFallback, expression);
    }

    private static bool IsSafe(Expression? expression)
        => expression is null // in method call results and constant values we trust to avoid too much conditions...
            || expression.NodeType == ExpressionType.Call
            || expression.NodeType == ExpressionType.Constant
            || !IsNullableOrReferenceType(expression.Type);

    private static Expression? Fallback(Type type)
    {
        // default values for generic collections
        if (type.IsGenericType && type.GetGenericArguments().Length == 1)
        {
            return CollectionFallback(typeof(List<>), type)
                ?? CollectionFallback(typeof(HashSet<>), type);
        }

        // default value for arrays
        return type.IsArray
            ? Expression.NewArrayInit(type.GetElementType()!)
            : null;
    }

    private static UnaryExpression? CollectionFallback(Type definition, Type type)
    {
        var collection = definition.MakeGenericType(type.GetGenericArguments());

        // try if an instance of this collection would suffice
        return type.IsAssignableFrom(collection)
            ? Expression.Convert(Expression.New(collection), type)
            : null;
    }

    private static bool IsExtensionMethod(MethodInfo element)
        => element.IsDefined(typeof(ExtensionAttribute), false);

    private static bool IsNullableOrReferenceType(Type type)
        => !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
}
