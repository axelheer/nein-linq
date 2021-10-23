using Xunit;

namespace NeinLinq.Tests;

public class RewriteEntityQueryCleanerTest
{
    private IQueryable<Model>? fieldQuery;

    private IQueryable<Model>? PropertyQuery { get; set; }

    [Fact]
    public void Visit_ResolvesAllTheInnerQueries()
    {
        var fieldRewriter = new TestExpressionVisitor();
        var otherFieldRewriter = new TestExpressionVisitor();
        var propertyRewriter = new TestExpressionVisitor();
        var rewriter = new TestExpressionVisitor();

        fieldQuery = CreateQuery().EntityRewrite(fieldRewriter).EntityRewrite(otherFieldRewriter);
        PropertyQuery = CreateQuery().EntityRewrite(propertyRewriter);

        var localQuery = from model in CreateQuery()
                         from fieldModel in fieldQuery
                         from propertyModel in PropertyQuery
                         select new
                         {
                             model,
                             fieldModel,
                             propertyModel
                         };

        var query = CreateQuery().EntityRewrite(rewriter).SelectMany(_ => localQuery);

        _ = query.ToList();

        Assert.True(rewriter.VisitCalled);
        Assert.True(propertyRewriter.VisitCalled);
        Assert.True(otherFieldRewriter.VisitCalled);
        Assert.True(fieldRewriter.VisitCalled);
    }

    private static IQueryable<Model> CreateQuery() => Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }

    private class TestExpressionVisitor : ExpressionVisitor
    {
        public bool VisitCalled { get; set; }

        public override Expression? Visit(Expression? node)
        {
            VisitCalled = true;
            return base.Visit(node);
        }
    }
}
