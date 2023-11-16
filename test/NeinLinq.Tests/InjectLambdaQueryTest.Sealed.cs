using Xunit;

namespace NeinLinq.Tests;

public class InjectLambdaQueryTest_Sealed
{
    [Fact]
    public void Query_WithoutSibling_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable(typeof(Functions)).Select(m => functions.VelocityWithoutSibling(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutSibling: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithConvention_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable(typeof(Functions)).Select(m => functions.VelocityWithConvention(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithMetadata_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithMethodMetadata_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithMethodMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithNullLambda_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithNullLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Lambda factory for VelocityWithNullLambda returns null.", error.Message);
    }

    [Fact]
    public void Query_WithoutLambda_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutLambda: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutExpression_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutExpression(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutExpression: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutSignature_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutSignature(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutSignature: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureType_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingSignatureType(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutMatchingSignatureType: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureSize_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingSignatureSize(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutMatchingSignatureSize: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithGenericArgument_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithGenericArgument(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithoutMatchingGenericArgument_Throws()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithoutMatchingGenericArgument(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Sealed+Functions.VelocityWithoutMatchingGenericArgument: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithPrivateSibling_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithPrivateSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
    }

    [Fact]
    public void Query_WithCachedExpression_Injects()
    {
        var functions = new Functions(2);

        var query = CreateQuery().ToInjectable().Select(m => functions.VelocityWithCachedExpression(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .12], result);
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

    private sealed class Functions
    {
        private readonly int digits;

        public Functions(int digits)
        {
            this.digits = digits;
        }

        public double VelocityWithoutSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public double VelocityWithConvention(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityWithConvention()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityWithMetadata()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda(nameof(Narf))]
        public double VelocityWithMethodMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> Narf()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithNullLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityWithNullLambda()
            => digits == 0 ? _ => 0 : null!;

        [InjectLambda]
        public double VelocityWithoutLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public IEnumerable<Func<Model, double>> VelocityWithoutLambda()
            => [v => Math.Round(v.Distance / v.Time, digits)];

        [InjectLambda]
        public double VelocityWithoutExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Func<Model, double> VelocityWithoutExpression()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithoutSignature(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public LambdaExpression VelocityWithoutSignature()
            => (Expression<Func<Model, double>>)(v => Math.Round(v.Distance / v.Time, digits));

        [InjectLambda]
        public double VelocityWithoutMatchingSignatureType(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, float>> VelocityWithoutMatchingSignatureType()
            => v => (float)Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithoutMatchingSignatureSize(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<double, double, double>> VelocityWithoutMatchingSignatureSize()
            => (d, t) => Math.Round(d / t, digits);

        [InjectLambda]
        public double VelocityWithGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<TModel, double>> VelocityWithGenericArgument<TModel>()
            where TModel : IModel
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithoutMatchingGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityWithoutMatchingGenericArgument()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithPrivateSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        private Expression<Func<Model, double>> VelocityWithPrivateSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda]
        public double VelocityWithCachedExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        private CachedExpression<Func<Model, double>> VelocityWithCachedExpressionExpr { get; }
            = CachedExpression.From<Func<Model, double>>(v => Math.Round(v.Distance / v.Time, 2));
    }
}
