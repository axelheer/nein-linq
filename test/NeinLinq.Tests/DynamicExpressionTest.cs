using System;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests;

public class DynamicExpressionTest
{
    [Fact]
    public void OperatorCreateComparison_NullArgument_Throws()
    {
        const string selector = "selector";
        const DynamicCompare comparer = DynamicCompare.Equal;

        var target = Expression.Parameter(typeof(Model));

        var targetError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateComparison(null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateComparison(target, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
            => DynamicExpression.CreateComparison(target, selector, (DynamicCompare)(object)-1, null));

        Assert.Equal("target", targetError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void MethodCreateComparison_NullArgument_Throws()
    {
        const string selector = "selector";
        const string comparer = "comparer";

        var target = Expression.Parameter(typeof(Model));

        var targetError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateComparison(null!, selector, comparer, null));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateComparison(target, null!, comparer, null));
        var comparerError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateComparison(target, selector, null!, null));

        Assert.Equal("target", targetError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
        Assert.Equal("comparer", comparerError.ParamName);
    }

    [Fact]
    public void CreateMemberAccess_NullArgument_Throws()
    {
        const string selector = "selector";

        var target = Expression.Parameter(typeof(Model));

        var targetError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateMemberAccess(null!, selector));
        var selectorError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.CreateMemberAccess(target, null!));

        Assert.Equal("target", targetError.ParamName);
        Assert.Equal("selector", selectorError.ParamName);
    }

    private class Model
    {
    }
}
