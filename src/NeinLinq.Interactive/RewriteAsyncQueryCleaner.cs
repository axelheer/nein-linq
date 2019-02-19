using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    internal class RewriteAsyncQueryCleaner : ExpressionVisitor
    {
        private static readonly MethodInfo rewriteQuery = typeof(RewriteAsyncQueryProvider).GetMethod("RewriteQuery");

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (typeof(IAsyncQueryable).IsAssignableFrom(node.Type))
            {
                var expression = Visit(node.Expression);

                if (expression is ConstantExpression target)
                {
                    var value = GetValue(target, node.Member);

                    while (value is RewriteAsyncQueryable rewrite)
                    {
                        value = rewriteQuery.MakeGenericMethod(rewrite.ElementType)
                            .Invoke(rewrite.Provider, new object[] { rewrite.Expression });
                    }

                    if (value is IAsyncQueryable query)
                    {
                        return query.Expression;
                    }
                }
            }

            return base.VisitMember(node);
        }

        private static object GetValue(ConstantExpression target, MemberInfo member)
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
