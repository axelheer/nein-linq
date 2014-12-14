using System;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Rebinds parameters and parameterized lambda expressions.
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
                var lambda = (LambdaExpression)replacement;

                // predicates have only one parameter...
                var t = lambda.Parameters[0];
                var u = node.Arguments[0];

                // ...which we replace with current argument
                var binder = new ParameterBinder(t, u);

                return binder.Visit(lambda.Body);
            }

            return base.VisitInvocation(node);
        }
    }
}
