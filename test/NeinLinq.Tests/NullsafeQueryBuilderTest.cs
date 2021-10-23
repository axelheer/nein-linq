using Xunit;

namespace NeinLinq.Tests;

public class NullsafeQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeQueryBuilder.ToNullsafe((IQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeQueryBuilder.ToNullsafe((IOrderedQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeQueryBuilder.ToNullsafe((IQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeQueryBuilder.ToNullsafe((IOrderedQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var result = value.ToNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var result = value.ToNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();

        var result = value.ToNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var result = value.ToNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }
}
