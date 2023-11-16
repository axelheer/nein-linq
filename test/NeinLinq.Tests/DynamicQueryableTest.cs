using System.Globalization;
using Xunit;

#pragma warning disable CA1305, CA1861

namespace NeinLinq.Tests;

[CLSCompliant(false)]
public class DynamicQueryableTest
{
    [Fact]
    public void UntypedOperatorWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const DynamicCompare comparer = DynamicCompare.Equal;

        var query = CreateQuery<IQueryable>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where((IQueryable)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
            => DynamicQueryable.Where(query, selector, (DynamicCompare)(object)-1, null));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void TypedOperatorWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const DynamicCompare comparer = DynamicCompare.Equal;

        var query = CreateQuery<IQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where((IQueryable<Model>)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
            => DynamicQueryable.Where(query, selector, (DynamicCompare)(object)-1, null));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void UntypedMethodWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const string comparer = "comparer";

        var query = CreateQuery<IQueryable>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where((IQueryable)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, selector, null!, null));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void TypedMethodWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const string comparer = "comparer";

        var query = CreateQuery<IQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where((IQueryable<Model>)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.Where(query, selector, null!, null));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, new[] { 5 })]
    [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
    [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
    public void UntypedOperatorWhere_ConcreteValue_Compares(DynamicCompare comparison, int[] expected)
    {
        var actual = CreateQuery<IQueryable>().Where("Number", comparison, "222,222", new CultureInfo("de-AT")).Cast<Model>().Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, new[] { 5 })]
    [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
    [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
    [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
    public void TypedOperatorWhere_ConcreteValue_Compares(DynamicCompare comparison, int[] expected)
    {
        var actual = CreateQuery<IQueryable<Model>>().Where("Number", comparison, "222,222", new CultureInfo("de-AT")).Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, 0)]
    [InlineData(DynamicCompare.NotEqual, 9)]
    [InlineData(DynamicCompare.GreaterThan, 0)]
    [InlineData(DynamicCompare.GreaterThanOrEqual, 0)]
    [InlineData(DynamicCompare.LessThan, 0)]
    [InlineData(DynamicCompare.LessThanOrEqual, 0)]
    public void UntypedOperatorWhere_NullValue_Compares(DynamicCompare comparison, int expected)
    {
        var actual = CreateQuery<IQueryable>().Where("Number", comparison, null).Cast<Model>().Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual.Length);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, 0)]
    [InlineData(DynamicCompare.NotEqual, 9)]
    [InlineData(DynamicCompare.GreaterThan, 0)]
    [InlineData(DynamicCompare.GreaterThanOrEqual, 0)]
    [InlineData(DynamicCompare.LessThan, 0)]
    [InlineData(DynamicCompare.LessThanOrEqual, 0)]
    public void TypedOperatorWhere_NullValue_Compares(DynamicCompare comparison, int expected)
    {
        var actual = CreateQuery<IQueryable<Model>>().Where("Number", comparison, null).Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual.Length);
    }

    [Fact]
    public void UntypedMethodWhere_Compares()
    {
        var actual = CreateQuery<IQueryable>().Where("Name", "Contains", "b").Cast<Model>().Select(d => d.Id).ToList();

        Assert.Equal([2, 5, 8], actual);
    }

    [Fact]
    public void TypedMethodWhere_Compares()
    {
        var actual = CreateQuery<IQueryable<Model>>().Where("Name", "Contains", "b").Select(d => d.Id).ToList();

        Assert.Equal([2, 5, 8], actual);
    }

    [Fact]
    public void UntypedOrderBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IQueryable>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.OrderBy((IQueryable)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.OrderBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public void TypedOrderBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.OrderBy((IQueryable<Model>)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.OrderBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public void UntypedThenBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IOrderedQueryable>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.ThenBy((IOrderedQueryable)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.ThenBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public void TypedThenBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IOrderedQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.ThenBy((IOrderedQueryable<Model>)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQueryable.ThenBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public void UntypedOrderByThenBy_Compares()
    {
        var one = CreateQuery<IQueryable>().OrderBy("Name.Length").ThenBy("Name", true);
        var two = CreateQuery<IQueryable>().OrderBy("Name.Length", true).ThenBy("Name");

        var oneResult = one.Cast<Model>().Select(d => d.Id).ToList();
        var twoResult = two.Cast<Model>().Select(d => d.Id).ToList();

        Assert.Equal([9, 8, 7, 6, 5, 4, 3, 2, 1], oneResult);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], twoResult);
    }

    [Fact]
    public void TypedOrderByThenBy_Compares()
    {
        var one = CreateQuery<IQueryable<Model>>().OrderBy("Name.Length").ThenBy("Name", true);
        var two = CreateQuery<IQueryable<Model>>().OrderBy("Name.Length", true).ThenBy("Name");

        var oneResult = one.Select(d => d.Id).ToList();
        var twoResult = two.Select(d => d.Id).ToList();

        Assert.Equal([9, 8, 7, 6, 5, 4, 3, 2, 1], oneResult);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], twoResult);
    }

    private static T CreateQuery<T>()
    {
        var data = new[]
        {
            new Model { Id = 1, Name = "aaaa", Number = 11.11m },
            new Model { Id = 2, Name = "bbbb", Number = 22.22m },
            new Model { Id = 3, Name = "cccc", Number = 33.33m },
            new Model { Id = 4, Name = "aaa", Number = 111.111m },
            new Model { Id = 5, Name = "bbb", Number = 222.222m },
            new Model { Id = 6, Name = "ccc", Number = 333.333m },
            new Model { Id = 7, Name = "aa", Number = 1111.1111m },
            new Model { Id = 8, Name = "bb", Number = 2222.2222m },
            new Model { Id = 9, Name = "cc", Number = 3333.3333m }
        };

        return (T)data.AsQueryable();
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? Number { get; set; }
    }
}
