using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    internal class RewriteQueryCleaner : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                var expression = Visit(node.Expression);

                if (expression is ConstantExpression target)
                {
                    var value = GetValue(target, node.Member);

                    while (value is RewriteQueryable rewrite)
                    {
                        value = rewrite.Provider.RewriteQuery(rewrite.Expression);
                    }

                    if (value is IQueryable query)
                    {
                        return query.Expression;
                    }
                }
            }

            return base.VisitMember(node);
        }

        private static object? GetValue(ConstantExpression target, MemberInfo member)
        {
            if (member is PropertyInfo p)
            {
                return p.GetValue(target.Value, null);
            }

            if (member is FieldInfo f)
            {
                return f.GetValue(target.Value);
            }

            return null;
        }
    }
}
