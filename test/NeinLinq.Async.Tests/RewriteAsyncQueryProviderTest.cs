using Xunit;

namespace NeinLinq.Tests;

public class RewriteAsyncQueryProviderTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();

        var providerError = Assert.Throws<ArgumentNullException>(()
            => new RewriteAsyncQueryProvider(null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => new RewriteAsyncQueryProvider(provider, null!));

        Assert.Equal("provider", providerError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void CreateQuery_CreatesRewriteAsyncQuery()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteAsyncQueryProvider(provider, rewriter);

        var result = subject.CreateQuery<Model>(Expression.Constant(true));

        Assert.Equal(rewriter, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Provider.Rewriter);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesVisitedAsync()
    {
        var provider = new TestQueryProvider();
        var rewriter = new TestExpressionVisitor();
        var subject = new RewriteAsyncQueryProvider(provider, rewriter);

        _ = await subject.ExecuteAsync<bool>(Expression.Constant(true), default);

        Assert.True(rewriter.VisitCalled);
    }

    private class Model
    {
    }

    private class TestQueryProvider : IAsyncQueryProvider
    {
        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => Enumerable.Empty<TElement>().ToAsyncEnumerable().AsAsyncQueryable();

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
            => new(default(TResult)!);
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
