using Xunit;

namespace NeinLinq.Tests;

public class InjectableQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable((IQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableQueryBuilder.ToInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable((IOrderedQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableQueryBuilder.ToInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable((IQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableQueryBuilder.ToInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable((IOrderedQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableQueryBuilder.ToInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableQueryBuilder.ToInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var result = value.ToInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var result = value.ToInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();

        var result = value.ToInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var result = value.ToInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }
}
