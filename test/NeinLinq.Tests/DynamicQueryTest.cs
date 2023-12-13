using System.Globalization;
using Xunit;

#pragma warning disable CA1861

namespace NeinLinq.Tests;

[CLSCompliant(false)]
public class DynamicQueryTest
{
    [Fact]
    public void OperatorCreatePredicate_NullArgument_Throws()
    {
        const string selector = "selector";
        const DynamicCompare comparer = DynamicCompare.Equal;

        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQuery.CreatePredicate<Model>(null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
            => DynamicQuery.CreatePredicate<Model>(selector, (DynamicCompare)(object)-1, null));

        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void MethodCreatePredicate_NullArgument_Throws()
    {
        const string selector = "selector";
        const string comparer = "comparer";

        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicQuery.CreatePredicate<Model>(null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentNullException>(()
            => DynamicQuery.CreatePredicate<Model>(selector, null!, null));

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
    public void OperatorCreatePredicate_ConcreteValue_Compares(DynamicCompare comparison, int[] expected)
    {
        var predicate = DynamicQuery.CreatePredicate<Model>("Number", comparison, "222,222", new CultureInfo("de-AT"));

        var actual = CreateQuery().Where(predicate).Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(DynamicCompare.Equal, 0)]
    [InlineData(DynamicCompare.NotEqual, 9)]
    [InlineData(DynamicCompare.GreaterThan, 0)]
    [InlineData(DynamicCompare.GreaterThanOrEqual, 0)]
    [InlineData(DynamicCompare.LessThan, 0)]
    [InlineData(DynamicCompare.LessThanOrEqual, 0)]
    public void OperatorCreatePredicate_NullValue_Compares(DynamicCompare comparison, int expected)
    {
        var predicate = DynamicQuery.CreatePredicate<Model>("Number", comparison, null);

        var actual = CreateQuery().Where(predicate).Select(d => d.Id).ToArray();

        Assert.Equal(expected, actual.Length);
    }

    [Fact]
    public void MethodCreatePredicate_Compares()
    {
        var predicate = DynamicQuery.CreatePredicate<Model>("Name", "Contains", "b");

        var result = CreateQuery().Where(predicate).Select(d => d.Id).ToList();

        Assert.Equal([2, 5, 8], result);
    }

    [Fact]
    public void OperatorCreatePredicate_GuidValue_Compares()
    {
        var predicate = DynamicQuery.CreatePredicate<Model>("Reference", DynamicCompare.Equal, Guid.NewGuid().ToString());

        var result = CreateQuery().Where(predicate).Select(d => d.Reference).ToArray();

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(nameof(OneTwo.Undefined), new[] { 1, 2, 3 })]
    [InlineData(nameof(OneTwo.One), new[] { 4, 5, 6 })]
    [InlineData(nameof(OneTwo.Two), new[] { 7, 8, 9 })]
    [InlineData("0", new[] { 1, 2, 3 })]
    [InlineData("1", new[] { 4, 5, 6 })]
    [InlineData("2", new[] { 7, 8, 9 })]
    public void OperatorCreatePredicate_EnumValue_Compares(string value, int[] expectedResult)
    {
        var predicate = DynamicQuery.CreatePredicate<Model>(nameof(Model.OneTwo), DynamicCompare.Equal, value);

        var result = CreateQuery().Where(predicate).Select(d => d.Id).ToArray();

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(nameof(OneTwo.Undefined), new[] { 4, 5 })]
    [InlineData(nameof(OneTwo.One), new[] { 7, 8 })]
    [InlineData(nameof(OneTwo.Two), new[] { 1, 2 })]
    [InlineData("0", new[] { 4, 5 })]
    [InlineData("1", new[] { 7, 8 })]
    [InlineData("2", new[] { 1, 2 })]
    [InlineData(null, new[] { 3, 6, 9 })]
    public void OperatorCreatePredicate_NullableEnumValue_Compares(string? value, int[] expectedResult)
    {
        var predicate = DynamicQuery.CreatePredicate<Model>(nameof(Model.OneTwoMaybe), DynamicCompare.Equal, value);

        var result = CreateQuery().Where(predicate).Select(d => d.Id).ToArray();

        Assert.Equal(expectedResult, result);
    }

    private static IQueryable<Model> CreateQuery()
    {
        var data = new[]
        {
            new Model { Id = 1, Name = "aaaa", Number = 11.11m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Undefined, OneTwoMaybe = OneTwo.Two },
            new Model { Id = 2, Name = "bbbb", Number = 22.22m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Undefined, OneTwoMaybe = OneTwo.Two},
            new Model { Id = 3, Name = "cccc", Number = 33.33m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Undefined, OneTwoMaybe = null},
            new Model { Id = 4, Name = "aaa", Number = 111.111m, Reference = Guid.NewGuid(), OneTwo = OneTwo.One, OneTwoMaybe = OneTwo.Undefined },
            new Model { Id = 5, Name = "bbb", Number = 222.222m, Reference = Guid.NewGuid(), OneTwo = OneTwo.One, OneTwoMaybe = OneTwo.Undefined },
            new Model { Id = 6, Name = "ccc", Number = 333.333m, Reference = Guid.NewGuid(), OneTwo = OneTwo.One, OneTwoMaybe = null },
            new Model { Id = 7, Name = "aa", Number = 1111.1111m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Two, OneTwoMaybe = OneTwo.One},
            new Model { Id = 8, Name = "bb", Number = 2222.2222m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Two, OneTwoMaybe = OneTwo.One },
            new Model { Id = 9, Name = "cc", Number = 3333.3333m, Reference = Guid.NewGuid(), OneTwo = OneTwo.Two, OneTwoMaybe = null }
        };

        return data.AsQueryable();
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? Number { get; set; }

        public Guid Reference { get; set; }

        public OneTwo OneTwo { get; set; }

        public OneTwo? OneTwoMaybe { get; set; }
    }

    public enum OneTwo
    {
        Undefined = 0,
        One = 1,
        Two = 2,
    }
}
