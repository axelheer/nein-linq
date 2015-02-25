using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Rebinds a parameter or a parameterized lambda expression.
    /// </summary>
    public class ParameterBinder : ExpressionVisitor
    {
        private readonly ParameterExpression parameter;
        private readonly Expression replacement;

        /// <summary>
        /// Creates a new parameter binder.
        /// </summary>
        /// <param name="parameter">The parameter to search for.</param>
        /// <param name="replacement">The expression to replace with.</param>
        public ParameterBinder(ParameterExpression parameter, Expression replacement)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (replacement == null)
                throw new ArgumentNullException("replacement");

            this.parameter = parameter;
            this.replacement = replacement;
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == null)
                return node;

            if (node == parameter)
                return replacement;

            return base.VisitParameter(node);
        }

        /// <inheritdoc />
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            if (node == null)
                return node;

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
