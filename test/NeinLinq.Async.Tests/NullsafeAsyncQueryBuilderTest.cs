using Xunit;

namespace NeinLinq.Tests;

public class NullsafeAsyncQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeAsyncQueryBuilder.ToAsyncNullsafe((IAsyncQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeAsyncQueryBuilder.ToAsyncNullsafe((IOrderedAsyncQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeAsyncQueryBuilder.ToAsyncNullsafe((IAsyncQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeAsyncQueryBuilder.ToAsyncNullsafe((IOrderedAsyncQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();

        var result = value.ToAsyncNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();

        var result = value.ToAsyncNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable>();

        var result = value.ToAsyncNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();

        var result = value.ToAsyncNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().ToAsyncEnumerable().AsAsyncQueryable();

    private class Model
    {
    }
}
