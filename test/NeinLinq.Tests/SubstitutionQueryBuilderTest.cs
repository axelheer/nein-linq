using Xunit;

namespace NeinLinq.Tests;

public class SubstitutionQueryBuilderTest
{
    [Fact]
    public void ToTypedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution((IQueryable<Model>)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToTypedOrderedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution((IOrderedQueryable<Model>)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToUntypedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution((IQueryable)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToUntypedOrderedQueryableSubstitution_NullArgument_Throws()
    {
        var value = CreateQuery<IOrderedQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var valueError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution((IOrderedQueryable)null!, from, to));
        var fromError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, null!, to));
        var toError = Assert.Throws<ArgumentNullException>(()
            => SubstitutionQueryBuilder.ToSubstitution(value, from, null!));

        Assert.Equal("value", valueError.ParamName);
        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    [Fact]
    public void ToTypedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToTypedOrderedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable<Model>>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    [Fact]
    public void ToUntypedOrderedQueryableSubstitution_RewritesQuery()
    {
        var value = CreateQuery<IOrderedQueryable>();
        var from = typeof(FromFunctions);
        var to = typeof(ToFunctions);

        var result = value.ToSubstitution(from, to);

        _ = Assert.IsType<SubstitutionQueryRewriter>(Assert.IsType<RewriteQueryProvider>(result.Provider).Rewriter);

        Assert.Equal(value, Assert.IsType<RewriteQueryable<Model>>(result).Query);
    }

    private static T CreateQuery<T>() => (T)Enumerable.Empty<Model>().AsQueryable();

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
