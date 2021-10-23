using Xunit;

namespace NeinLinq.Tests;

public class NullsafeEntityQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeEntityQueryBuilder.ToEntityNullsafe((IQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeEntityQueryBuilder.ToEntityNullsafe((IOrderedQueryable<Model>)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeEntityQueryBuilder.ToEntityNullsafe((IQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_NullArgument_Throws()
    {
        var valueError = Assert.Throws<ArgumentNullException>(()
            => NullsafeEntityQueryBuilder.ToEntityNullsafe((IOrderedQueryable)null!));

        Assert.Equal("value", valueError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var result = value.ToEntityNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var result = value.ToEntityNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();

        var result = value.ToEntityNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableNullsafe_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var result = value.ToEntityNullsafe();

        _ = Assert.IsType<NullsafeQueryRewriter>(Assert.IsType<EntityQueryProviderAdapter>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteEntityQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }
}
