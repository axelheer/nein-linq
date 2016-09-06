using System.Linq.Expressions;

namespace NeinLinq.Tests.FakeAsyncQuery
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
