using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    sealed class ParameterBinder : ExpressionVisitor
    {
        readonly ParameterExpression parameter;
        readonly Expression replacement;

        public ParameterBinder(ParameterExpression parameter, Expression replacement)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            if (replacement == null)
                throw new ArgumentNullException(nameof(replacement));

            this.parameter = parameter;
            this.replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node == parameter)
            {
                return replacement;
            }

            return base.VisitParameter(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Expression == parameter)
            {
                var lambda = replacement as LambdaExpression;

                if (lambda != null)
                {
                    var binders = lambda.Parameters.Zip(node.Arguments,
                        (p, a) => new ParameterBinder(p, a));

                    return binders.Aggregate(lambda.Body, (e, b) => b.Visit(e));
                }
            }

            return base.VisitInvocation(node);
        }
    }
}
