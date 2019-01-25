using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    /// <summary>
    /// Resolves rewriteable subqueries.
    /// </summary>
    public  class RewriteAsyncQueryCleaner : ExpressionVisitor
    {
        private static readonly MethodInfo rewriteQuery = typeof(RewriteAsyncQueryProvider).GetMethod("RewriteQuery");

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Expression is ConstantExpression expression)
            {
                var value = GetValue(expression, node.Member);

                while (value is RewriteAsyncQueryable query)
                {
                    value = rewriteQuery.MakeGenericMethod(query.ElementType)
                        .Invoke(query.Provider, new object[] { query.Expression });
                }

                return Expression.Constant(value, node.Type);
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
