using Xunit;

namespace NeinLinq.Tests;

public class RewriteEntityQueryProviderTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();

        var providerError = Assert.Throws<ArgumentNullException>(()
            => new RewriteEntityQueryProvider(null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => new RewriteEntityQueryProvider(provider, null!));

        Assert.Equal("provider", providerError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void TypedCreateQuery_CreatesRewriteEntityQuery()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        var result = subject.CreateQuery<Model>(Expression.Constant(true));

        Assert.Equal(rewriter, Assert.IsType<RewriteEntityQueryable<Model>>(result).Provider.Rewriter);
    }

    [Fact]
    public void UntypedCreateQuery_CreatesRewriteEntityQuery()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        var result = subject.CreateQuery(Expression.Constant(true));

        Assert.Equal(rewriter, Assert.IsType<RewriteEntityQueryable<Model>>(result).Provider.Rewriter);
    }

    [Fact]
    public void TypedExecute_ExecutesVisited()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        _ = subject.Execute<bool>(Expression.Constant(true));

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedExecute_ExecutesVisited()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        _ = subject.Execute(Expression.Constant(true));

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void AsyncTypedExecute_ExecutesVisited()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        _ = subject.ExecuteAsync<bool>(Expression.Constant(true), default);

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public async Task AsyncTypedExecute_ExecutesVisitedAsync()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteEntityQueryProvider(provider, rewriter);

        _ = await subject.ExecuteAsync<Task<bool>>(Expression.Constant(true), default);

        Assert.True(rewriter.VisitCalled);
    }

    private class Model
    {
    }

    private class TestQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
            => Enumerable.Empty<Model>().AsQueryable();

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => Enumerable.Empty<TElement>().AsQueryable();

        public object? Execute(Expression expression)
            => null;

        public TResult Execute<TResult>(Expression expression)
            => default!;
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
