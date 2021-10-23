using Xunit;

namespace NeinLinq.Tests;

public class SubstitutionQueryRewriterTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var fromError = Assert.Throws<ArgumentNullException>(()
            => new SubstitutionQueryRewriter(null!, typeof(ToFunctions)));
        var toError = Assert.Throws<ArgumentNullException>(()
            => new SubstitutionQueryRewriter(typeof(FromFunctions), null!));

        Assert.Equal("from", fromError.ParamName);
        Assert.Equal("to", toError.ParamName);
    }

    private static class FromFunctions
    {
    }

    private static class ToFunctions
    {
    }
}
