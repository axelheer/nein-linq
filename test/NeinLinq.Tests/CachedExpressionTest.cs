using Xunit;

namespace NeinLinq.Tests;

public class CachedExpressionTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var expressionError = Assert.Throws<ArgumentNullException>(()
            => new CachedExpression<Func<int, int>>(null!));

        Assert.Equal("expression", expressionError.ParamName);
    }

    [Fact]
    public void Compiled_IncrementExpression_Increments()
    {
        Expression<Func<int, int>> expression = i => i + i;

        var subject = CachedExpression.From(expression);

        Assert.Equal(2, subject.Compiled(1));
    }

    [Fact]
    public void ToTypedLambdaExpression_ProvicesActualExpression()
    {
        Expression<Func<int, int>> expected = i => i + i;

        var subject = CachedExpression.From(expected);

        var actual = (Expression<Func<int, int>>)subject;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToUntypedLambdaExpression_ProvicesActualExpression()
    {
        Expression<Func<int, int>> expected = i => i + i;

        var subject = CachedExpression.From(expected);

        var actual = (LambdaExpression)subject;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToTypedLambdaExpression_NullArgument_ReturnsNull()
    {
        var subject = default(CachedExpression<Func<int, int>>);

        var actual = (Expression<Func<int, int>>)subject!;

        Assert.Null(actual);
    }

    [Fact]
    public void ToUntypedLambdaExpression_NullArgument_ReturnsNull()
    {
        var subject = default(CachedExpression<Func<int, int>>);

        var actual = (LambdaExpression)subject!;

        Assert.Null(actual);
    }
}
