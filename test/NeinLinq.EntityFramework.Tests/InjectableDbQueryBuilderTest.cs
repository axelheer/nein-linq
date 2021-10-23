using Xunit;

namespace NeinLinq.Tests;

public class InjectableDbQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable((IQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable((IOrderedQueryable<Model>)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable((IQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var valueError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable((IOrderedQueryable)null!));
        var greenlistError = Assert.Throws<ArgumentNullException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, null!));
        var greenlistRangeError = Assert.Throws<ArgumentOutOfRangeException>(()
            => InjectableDbQueryBuilder.ToDbInjectable(value, typeof(Model), null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("greenlist", greenlistError.ParamName);
        Assert.Equal("greenlist", greenlistRangeError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();

        var result = value.ToDbInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteDbQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteDbQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();

        var result = value.ToDbInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteDbQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteDbQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();

        var result = value.ToDbInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteDbQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteDbQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableInjectable_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();

        var result = value.ToDbInjectable();

        _ = Assert.IsType<InjectableQueryRewriter>(Assert.IsType<RewriteDbQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteDbQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

    private class Model
    {
    }
}
