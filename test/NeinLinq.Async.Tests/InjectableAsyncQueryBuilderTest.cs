using Xunit;

namespace NeinLinq.Tests;

public class InjectableAsyncQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable((IAsyncQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable((IOrderedAsyncQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable((IAsyncQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable((IOrderedAsyncQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableAsyncQueryBuilder.ToAsyncInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();

        var result = value.ToAsyncInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();

        var result = value.ToAsyncInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable>();

        var result = value.ToAsyncInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();

        var result = value.ToAsyncInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().ToAsyncEnumerable().AsAsyncQueryable();

    private class Model
    {
    }
}
