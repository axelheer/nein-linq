using Xunit;

namespace NeinLinq.Tests;

public class RewriteAsyncQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite((IAsyncQueryable<Model>)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite((IOrderedAsyncQueryable<Model>)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite((IAsyncQueryable)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite((IOrderedAsyncQueryable)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteAsyncQueryBuilder.AsyncRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var result = value.AsyncRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var result = value.AsyncRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable>();
        var rewriter = new TestExpressionVisitor();

        var result = value.AsyncRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();
        var rewriter = new TestExpressionVisitor();

        var result = value.AsyncRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().ToAsyncEnumerable().AsAsyncQueryable();

    private class Model
    {
    }

    private class TestExpressionVisitor : ExpressionVisitor
    {
    }
}
