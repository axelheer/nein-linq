using System.Reflection;

namespace NeinLinq;

/// <summary>
/// Prepares queries to be rewritten.
/// </summary>
public class RewriteQueryCleaner : ExpressionVisitor
{
    /// <inheritdoc />
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (typeof(IQueryable).IsAssignableFrom(node.Type))
        {
            var expression = Visit(node.Expression);
            if (expression is ConstantExpression target)
            {
                var value = GetValue(target, node.Member);
                while (value is RewriteQueryable rewrite)
                    value = rewrite.Provider.RewriteQuery(rewrite.Expression);
                if (value is IQueryable query)
                    return Visit(query.Expression);
            }
        }

        return base.VisitMember(node);
    }

    private static object? GetValue(ConstantExpression target, MemberInfo member) => member switch
    {
        PropertyInfo p => p.GetValue(target.Value, null),
        FieldInfo f => f.GetValue(target.Value),
        _ => null
    };
}
