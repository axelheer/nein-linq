using Xunit;

namespace NeinLinq.Tests;

public class RewriteEntityQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite((IQueryable<Model>)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite((IOrderedQueryable<Model>)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite((IQueryable)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableRewrite_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable>();
        var rewriter = new TestExpressionVisitor();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite((IOrderedQueryable)null!, rewriter));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteEntityQueryBuilder.EntityRewrite(value, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var result = value.EntityRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();
        var rewriter = new TestExpressionVisitor();

        var result = value.EntityRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();
        var rewriter = new TestExpressionVisitor();

        var result = value.EntityRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableRewrite_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();
        var rewriter = new TestExpressionVisitor();

        var result = value.EntityRewrite(rewriter);

        _ = Assert.IsType<TestExpressionVisitor>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }

    private class TestExpressionVisitor : ExpressionVisitor
    {
    }
}
