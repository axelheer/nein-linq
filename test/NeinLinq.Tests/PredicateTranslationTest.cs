using Xunit;

namespace NeinLinq.Tests;

public class PredicateTranslationTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(()
            => new PredicateTranslation<Model>(null!));

        Assert.Equal("predicate", error.ParamName);
    }

    private class Model
    {
    }
}
