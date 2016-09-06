using System.Linq.Expressions;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Rewriter : ExpressionVisitor
    {
        public bool VisitCalled { get; set; }

        public override Expression Visit(Expression node)
        {
            VisitCalled = true;

            return base.Visit(node);
        }
    }
}
