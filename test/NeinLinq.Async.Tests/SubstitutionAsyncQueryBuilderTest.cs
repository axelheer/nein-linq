using Xunit;

namespace NeinLinq.Tests;

public class SubstitutionAsyncQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution((IAsyncQueryable<Model>)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution((IOrderedAsyncQueryable<Model>)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IAsyncQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution((IAsyncQueryable)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution((IOrderedAsyncQueryable)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionAsyncQueryBuilder.ToAsyncSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToAsyncSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToAsyncSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IAsyncQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToAsyncSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IOrderedAsyncQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToAsyncSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteAsyncQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteAsyncQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().ToAsyncEnumerable().AsAsyncQueryable();

    private class Model
    {
    }

    private static class FromFunctions
    {
    }

    private static class ToFunctions
    {
    }
}
