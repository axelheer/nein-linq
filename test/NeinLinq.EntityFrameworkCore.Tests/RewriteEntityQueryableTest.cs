using Xunit;

namespace NeinLinq.Tests;

public class RewriteEntityQueryableTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var query = CreateQuery();
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteEntityQueryProvider(query.Provider, rewriter);

        var queryError = Assert.Throws<ArgumentNullException>(()
            => new RewriteEntityQueryable<Model>(null!, provider));
        var providerError = Assert.Throws<ArgumentNullException>(()
            => new RewriteEntityQueryable<Model>(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("provider", providerError.ParamName);
    }

    [Fact]
    public void TypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteEntityQuery(query);

        _ = subject.GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteEntityQuery(query);

        _ = ((IEnumerable)subject).GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void GetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteEntityQuery(query);

        _ = subject.GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void ElementType_ReturnsElementType()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteEntityQuery(query);

        Assert.Equal(query.ElementType, subject.ElementType);
    }

    [Fact]
    public void Expression_ReturnsExpression()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteEntityQuery(query);

        Assert.Equal(query.Expression, subject.Expression);
    }

    [Fact]
    public void Provider_ReturnsProvider()
    {
        var query = CreateQuery();

        var (_, provider, subject) = CreateRewriteEntityQuery(query);

        Assert.Equal(provider, subject.Provider);
    }

    [Fact]
    public void QueryableProvider_ReturnsProviderAdapter()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteEntityQuery(query);

        Assert.Equal(rewriter, Assert.IsType<EntityQueryProviderAdapter>(((IQueryable)subject).Provider).Rewriter);
    }

    [Fact]
    public void Query_ReturnsQuery()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteEntityQuery(query);

        Assert.Equal(query, subject.Query);
    }

    private static (TestExpressionVisitor, RewriteEntityQueryProvider, RewriteEntityQueryable<Model>) CreateRewriteEntityQuery(IQueryable<Model> query)
    {
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteEntityQueryProvider(query.Provider, rewriter);

        return (rewriter, provider, new RewriteEntityQueryable<Model>(query, provider));
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
