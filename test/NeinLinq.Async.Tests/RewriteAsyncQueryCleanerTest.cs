using Xunit;

namespace NeinLinq.Tests;

public class RewriteAsyncQueryCleanerTest
{
    private IAsyncQueryable<Model>? fieldQuery;

    private IAsyncQueryable<Model>? PropertyQuery { get; set; }

    [Fact]
    public async Task Visit_ResolvesAllTheInnerQueriesAsync()
    {
        var fieldRewriter = new TestExpressionVisitor();
        var otherFieldRewriter = new TestExpressionVisitor();
        var propertyRewriter = new TestExpressionVisitor();
        var rewriter = new TestExpressionVisitor();

        fieldQuery = CreateQuery().AsyncRewrite(fieldRewriter).AsyncRewrite(otherFieldRewriter);
        PropertyQuery = CreateQuery().AsyncRewrite(propertyRewriter);

        var localQuery = from model in CreateQuery()
                         from fieldModel in fieldQuery
                         from propertyModel in PropertyQuery
                         select new
                         {
                             model,
                             fieldModel,
                             propertyModel
                         };

        var query = CreateQuery().AsyncRewrite(rewriter).SelectMany(_ => localQuery);

        _ = await query.ToListAsync();

        Assert.True(rewriter.VisitCalled);
        Assert.True(propertyRewriter.VisitCalled);
        Assert.True(otherFieldRewriter.VisitCalled);
        Assert.True(fieldRewriter.VisitCalled);
    }

    private static IAsyncQueryable<Model> CreateQuery() => Enumerable.Empty<Model>().ToAsyncEnumerable().AsAsyncQueryable();

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
