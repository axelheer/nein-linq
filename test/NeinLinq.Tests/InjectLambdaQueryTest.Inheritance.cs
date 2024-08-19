using Xunit;

namespace NeinLinq.Tests;

public class InjectLambdaQueryTest_Inheritance
{
    [Fact]
    public void Query_WithoutSibling_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable(typeof(FunctionsBase)).Select(m => functions.VelocityWithoutSibling(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutSibling: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithConvention_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable(typeof(FunctionsBase)).Select(m => functions.VelocityWithConvention(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithMetadata_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithMethodMetadata_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithMethodMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithNullLambda_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithNullLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Lambda factory for VelocityWithNullLambda returns null.", error.Message);
    }

    [Fact]
    public void Query_WithoutLambda_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutLambda: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutExpression_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutExpression(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutExpression: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutSignature_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutSignature(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutSignature: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureType_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingSignatureType(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutMatchingSignatureType: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureSize_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingSignatureSize(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutMatchingSignatureSize: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithGenericArgument_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithGenericArgument(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithoutMatchingGenericArgument_Throws()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingGenericArgument(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Inheritance+Functions.VelocityWithoutMatchingGenericArgument: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithPrivateSibling_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithPrivateSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithHiddenSiblingViaBase_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithHiddenSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithHiddenSiblingViaDerived_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithHiddenSibling(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Implementing sibling has been hidden.", error.Message);
    }

    [Fact]
    public void Query_WithAbstractSiblingViaBase_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithAbstractSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithPrivateBase_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = functions.CallVelocityWithPrivateBase(CreateQuery()).ToInjectable();

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithAbstractSiblingViaDerived_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithAbstractSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithVirtualSiblingViaBase_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithVirtualSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithVirtualSiblingViaDerived_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithVirtualSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithCachedExpression_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithCachedExpression(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithHiddenStaticSiblingViaBase_Injects()
    {
        FunctionsBase functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithHiddenStaticSibling(m));

        var result = query.ToList();

        Assert.Equal([200, 0, 0.125], result);
    }

    [Fact]
    public void Query_WithHiddenStaticSiblingViaDerived_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithHiddenStaticSibling(m));

        var result = query.ToList();

        Assert.Equal([200, 0, 0.125], result);
    }

    private static IQueryable<Model> CreateQuery()
    {
        var data = new[]
        {
            new Model { Id = 1, Name = "Asdf", Distance = 66, Time = .33 },
            new Model { Id = 2, Name = "Narf", Distance = 0, Time = 3.14 },
            new Model { Id = 3, Name = "Qwer", Distance = 8, Time = 64 }
        };

        return data.AsQueryable();
    }

    private interface IModel
    {
        int Id { get; set; }

        string Name { get; set; }

        double Distance { get; set; }

        double Time { get; set; }
    }

    private class Model : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double Distance { get; set; }

        public double Time { get; set; }
    }

    private abstract class FunctionsBase
    {
        private readonly int digits;

        protected FunctionsBase(int digits)
        {
            this.digits = digits;
        }

        public double VelocityWithoutSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public double VelocityWithConvention(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda("ignore-me")]
        public abstract double VelocityWithMethodMetadata(Model value);

        [InjectLambda]
        public double VelocityWithNullLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutSignature(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutMatchingSignatureType(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutMatchingSignatureSize(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithoutMatchingGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithPrivateSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithHiddenSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityWithHiddenSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithAbstractSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public IQueryable<double> CallVelocityWithPrivateBase(IQueryable<Model> query)
            => query.Select(m => VelocityWithPrivateBase(m));

        [InjectLambda]
        private double VelocityWithPrivateBase(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public abstract Expression<Func<Model, double>> VelocityWithAbstractSibling();

        [InjectLambda]
        public double VelocityWithVirtualSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public virtual Expression<Func<Model, double>> VelocityWithVirtualSibling()
            => throw new InvalidOperationException("Implementing sibling is missing.");

        [InjectLambda]
        public double VelocityWithCachedExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda]
        public double VelocityWithHiddenStaticSibling(Model value)
           => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityWithHiddenStaticSibling()
            => throw new InvalidOperationException("Implementing sibling is missing.");
    }

    private class Functions : FunctionsBase
    {
        private readonly int digits;

        public Functions(int digits)
            : base(digits)
        {
            this.digits = digits;
        }

        public Expression<Func<Model, double>> VelocityWithConvention()
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Model, double>> VelocityWithMetadata()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda(nameof(Narf))]
        public override double VelocityWithMethodMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> Narf()
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Model, double>> VelocityWithNullLambda()
            => digits == 0 ? _ => 0 : null!;

        public IEnumerable<Func<Model, double>> VelocityWithoutLambda()
            => [v => Math.Round(v.Distance / v.Time, digits)];

        public Func<Model, double> VelocityWithoutExpression()
            => v => Math.Round(v.Distance / v.Time, digits);

        public LambdaExpression VelocityWithoutSignature()
            => (Expression<Func<Model, double>>)(v => Math.Round(v.Distance / v.Time, digits));

        public Expression<Func<Model, float>> VelocityWithoutMatchingSignatureType()
            => v => (float)Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<double, double, double>> VelocityWithoutMatchingSignatureSize()
            => (d, t) => Math.Round(d / t, digits);

        public Expression<Func<TModel, double>> VelocityWithGenericArgument<TModel>()
            where TModel : IModel
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Model, double>> VelocityWithoutMatchingGenericArgument()
            => v => Math.Round(v.Distance / v.Time, digits);

        private Expression<Func<Model, double>> VelocityWithPrivateSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        public new Expression<Func<Model, double>> VelocityWithHiddenSibling()
            => digits == 0 ? base.VelocityWithHiddenSibling() : throw new InvalidOperationException("Implementing sibling has been hidden.");

        public override Expression<Func<Model, double>> VelocityWithAbstractSibling()
            => v => Math.Round(v.Distance / v.Time, digits);
        public Expression<Func<Model, double>> VelocityWithPrivateBase()
            => v => Math.Round(v.Distance / v.Time, digits);

        public override Expression<Func<Model, double>> VelocityWithVirtualSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        private CachedExpression<Func<Model, double>> VelocityWithCachedExpressionExpr { get; }
            = CachedExpression.From<Func<Model, double>>(v => Math.Round(v.Distance / v.Time, 2));

        public new static Expression<Func<Model, double>> VelocityWithHiddenStaticSibling()
            => v => v.Distance / v.Time;
    }
}
