using Xunit;

#pragma warning disable CA1305

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

    [Fact]
    public void RegisterConverter_NullArgument_Throws()
    {
        var type = typeof(CustomType);

        Func<string, IFormatProvider?, object> converter = (value, _) => CustomType.Create(value);

        var typeError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.RegisterConverter(null!, converter));
        var converterError = Assert.Throws<ArgumentNullException>(()
            => DynamicExpression.RegisterConverter(type, null!));

        Assert.Equal("type", typeError.ParamName);
        Assert.Equal("converter", converterError.ParamName);
    }

    [Fact]
    public void RegisterConverter_ValidArguments_Works()
    {
        DynamicExpression.RegisterConverter(typeof(CustomType), (value, _) => CustomType.Create(value));

        var predicate = DynamicQuery.CreatePredicate<Model>("Value", DynamicCompare.Equal, "narf");

        var actual = Assert.IsType<CustomType>(
            Assert.IsAssignableFrom<ConstantExpression>(
                Assert.IsAssignableFrom<BinaryExpression>(
                    predicate.Body)
                .Right)
            .Value)
        .Value;

        Assert.Equal("narf", actual);
    }

    private class CustomType
    {
        public string Value { get; private set; } = "";

        public static CustomType Create(string value)
            => new() { Value = value };
    }

    private class Model
    {
        public CustomType? Value { get; set; }
    }
}
