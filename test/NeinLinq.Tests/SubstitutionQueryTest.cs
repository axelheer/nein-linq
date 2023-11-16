using Xunit;

namespace NeinLinq.Tests;

public class SubstitutionQueryTest
{
    [Fact]
    public void Query_WithSubstitution_CallsSubstitute()
    {
        var query = CreateQuery().ToSubstitution(typeof(FromFunctions), typeof(ToFunctions));

        var result = query.Where(_ => FromFunctions.Condition()).Count();

        Assert.Equal(0, result);
    }

    [Fact]
    public void Query_WithoutSubstitution_CallsOriginal()
    {
        var query = CreateQuery();

        var result = query.Where(_ => FromFunctions.Condition()).Count();

        Assert.Equal(1, result);
    }

    private static IQueryable<Model> CreateQuery()
    {
        var items = new[]
        {
            new Model()
        };

        return items.AsQueryable();
    }

    private class Model
    {
    }

    private static class FromFunctions
    {
        public static bool Condition() => true;
    }

    private static class ToFunctions
    {
        public static bool Condition() => false;
    }
}
