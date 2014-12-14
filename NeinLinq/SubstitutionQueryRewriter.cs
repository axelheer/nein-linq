using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for replacing method types.
    /// </summary>
    /// <remarks>
    /// Use <see cref="SubstitutionQueryBuilder" /> to make a query substitution.
    /// </remarks>
    public class SubstitutionQueryRewriter : ExpressionVisitor
    {
        private readonly Type from;
        private readonly Type to;

        /// <summary>
        /// Creates a new substitution query rewriter.
        /// </summary>
        /// <param name="from">A type to replace.</param>
        /// <param name="to">A type to use instead.</param>
        public SubstitutionQueryRewriter(Type from, Type to)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            this.from = from;
            this.to = to;
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null || node.Method == null)
                return node;

            if (node.Method.DeclaringType == from)
            {
                var typeArguments = node.Method.GetGenericArguments();
                var arguments = node.Arguments.Select(a => Visit(a)).ToArray();

                // assume equivalent method signature
                return Expression.Call(to, node.Method.Name, typeArguments, arguments);
            }

            return base.VisitMethodCall(node);
        }
    }
}
