using Xunit;

namespace NeinLinq.Tests;

public class ParameterBinderTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var parameterError = Assert.Throws<ArgumentNullException>(()
            => new ParameterBinder(null!, Expression.Constant(true)));
        var replacementError = Assert.Throws<ArgumentNullException>(()
            => new ParameterBinder(Expression.Parameter(typeof(bool)), null!));

        Assert.Equal("parameter", parameterError.ParamName);
        Assert.Equal("replacement", replacementError.ParamName);
    }

    [Fact]
    public void Visit_MatchingParameter_ProvidesReplacement()
    {
        var parameter = Expression.Parameter(typeof(bool));
        var replacement = Expression.Constant(true);

        var result = new ParameterBinder(parameter, replacement).Visit(parameter);

        Assert.Equal(replacement, result);
    }

    [Fact]
    public void Visit_NonMatchingParameter_LeavesUnchanged()
    {
        var parameter = Expression.Parameter(typeof(bool));
        var replacement = Expression.Constant(true);
        var otherParameter = Expression.Parameter(typeof(bool));

        var result = new ParameterBinder(otherParameter, replacement).Visit(parameter);

        Assert.Equal(parameter, result);
    }

    [Fact]
    public void Visit_MatchingLambdaParameter_ProvidesReplacement()
    {
        var lambdaParameter = Expression.Parameter(typeof(Func<bool, bool>));
        var parameter = Expression.Parameter(typeof(bool));
        var lambdaReplacement = Expression.Lambda<Func<bool, bool>>(parameter, parameter);
        var replacement = Expression.Parameter(typeof(bool));
        var invocation = Expression.Invoke(lambdaParameter, replacement);

        var result = new ParameterBinder(lambdaParameter, lambdaReplacement).Visit(invocation);

        Assert.Equal(replacement, result);
    }

    [Fact]
    public void Visit_NonMatchingLambdaParameter_LeavesUnchanged()
    {
        var lambdaParameter = Expression.Parameter(typeof(Func<bool, bool>));
        var parameter = Expression.Parameter(typeof(bool));
        var lambdaReplacement = Expression.Lambda<Func<bool, bool>>(parameter, parameter);
        var replacement = Expression.Parameter(typeof(bool));
        var invocation = Expression.Invoke(lambdaParameter, replacement);
        var otherLambdaParameter = Expression.Parameter(typeof(Func<bool, bool>));

        var result = new ParameterBinder(otherLambdaParameter, lambdaReplacement).Visit(invocation);

        Assert.Equal(invocation, result);
    }

    [Fact]
    public void Visit_MatchingDelegateReplacement_ProvidesReplacement()
    {
        var lambdaParameter = Expression.Parameter(typeof(Func<bool>));
        var lambdaReplacement = Expression.Parameter(typeof(Func<bool>));
        var invocation = Expression.Invoke(lambdaParameter);

        var result = new ParameterBinder(lambdaParameter, lambdaReplacement).Visit(invocation);

        Assert.Equal(lambdaReplacement, Assert.IsAssignableFrom<InvocationExpression>(result).Expression);
    }
}
