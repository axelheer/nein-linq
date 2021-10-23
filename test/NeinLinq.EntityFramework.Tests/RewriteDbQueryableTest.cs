using System.Data.Entity.Infrastructure;
using Xunit;

namespace NeinLinq.Tests;

public class RewriteDbQueryableTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var query = CreateQuery();
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteDbQueryProvider(query.Provider, rewriter);

        var queryError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryable<Model>(null!, provider));
        var providerError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryable<Model>(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("provider", providerError.ParamName);
    }

    [Fact]
    public void TypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = subject.GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = ((IEnumerable)subject).GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void TypedGetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = subject.GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedGetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = ((IDbAsyncEnumerable)subject).GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void ElementType_ReturnsElementType()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query.ElementType, subject.ElementType);
    }

    [Fact]
    public void Expression_ReturnsExpression()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query.Expression, subject.Expression);
    }

    [Fact]
    public void Provider_ReturnsProvider()
    {
        var query = CreateQuery();

        var (_, provider, subject) = CreateRewriteQuery(query);

        Assert.Equal(provider, subject.Provider);
    }

    [Fact]
    public void QueryableProvider_ReturnsProvider()
    {
        var query = CreateQuery();

        var (_, provider, subject) = CreateRewriteQuery(query);

        Assert.Equal(provider, ((IQueryable)subject).Provider);
    }

    [Fact]
    public void Query_ReturnsQuery()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query, subject.Query);
    }

    private static (TestExpressionVisitor, RewriteDbQueryProvider, RewriteDbQueryable<Model>) CreateRewriteQuery(IQueryable<Model> query)
    {
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteDbQueryProvider(query.Provider, rewriter);

        return (rewriter, provider, new RewriteDbQueryable<Model>(query, provider));
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
