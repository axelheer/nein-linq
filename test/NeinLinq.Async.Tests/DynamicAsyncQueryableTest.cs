using System.Globalization;
using Xunit;

#pragma warning disable CA1861

namespace NeinLinq.Tests;

[CLSCompliant(false)]
public class DynamicAsyncQueryableTest
{
    [Fact]
    public void OperatorWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const DynamicCompare comparer = DynamicCompare.Equal;

        var query = CreateQuery<IAsyncQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.Where((IAsyncQueryable<Model>)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
            => DynamicAsyncQueryable.Where(query, selector, (DynamicCompare)(object)-1, null));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void MethodWhere_NullArgument_Throws()
    {
        const string selector = "selector";
        const string comparer = "comparer";

        var query = CreateQuery<IAsyncQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.Where((IAsyncQueryable<Model>)null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.Where(query, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.Where(query, selector, null!, null));

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
    public async Task OperatorWhere_ConcreteValue_ComparesAsync(DynamicCompare comparison, int[] expected)
    {
        var actual = await CreateQuery<IAsyncQueryable<Model>>().Where("Number", comparison, "222,222", new CultureInfo("de-AT")).Select(d => d.Id).ToArrayAsync();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, 0)]
    [InlineData(DynamicCompare.NotEqual, 9)]
    [InlineData(DynamicCompare.GreaterThan, 0)]
    [InlineData(DynamicCompare.GreaterThanOrEqual, 0)]
    [InlineData(DynamicCompare.LessThan, 0)]
    [InlineData(DynamicCompare.LessThanOrEqual, 0)]
    public async Task OperatorWhere_NullValue_ComparesAsync(DynamicCompare comparison, int expected)
    {
        var actual = await CreateQuery<IAsyncQueryable<Model>>().Where("Number", comparison, null).Select(d => d.Id).ToArrayAsync();

        Assert.Equal(expected, actual.Length);
    }

    [Fact]
    public async Task MethodWhere_Compares()
    {
        var actual = await CreateQuery<IAsyncQueryable<Model>>().Where("Name", "Contains", "b").Select(d => d.Id).ToListAsync();

        Assert.Equal([2, 5, 8], actual);
    }

    [Fact]
    public void OrderBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IAsyncQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.OrderBy((IAsyncQueryable<Model>)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.OrderBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public void ThenBy_NullArgument_Throws()
    {
        const string selector = "selector";

        var query = CreateQuery<IOrderedAsyncQueryable<Model>>();

        var queryError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.ThenBy((IOrderedAsyncQueryable<Model>)null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicAsyncQueryable.ThenBy(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    [Fact]
    public async Task OrderByThenBy_ComparesAsync()
    {
        var one = CreateQuery<IAsyncQueryable<Model>>().OrderBy("Name.Length").ThenBy("Name", true);
        var two = CreateQuery<IAsyncQueryable<Model>>().OrderBy("Name.Length", true).ThenBy("Name");

        var oneResult = await one.Select(d => d.Id).ToListAsync();
        var twoResult = await two.Select(d => d.Id).ToListAsync();

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

        return (T)data.ToAsyncEnumerable().AsAsyncQueryable();
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? Number { get; set; }
    }
}
