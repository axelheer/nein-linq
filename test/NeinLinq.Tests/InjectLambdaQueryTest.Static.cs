using Xunit;

namespace NeinLinq.Tests;

public class InjectLambdaQueryTest_Static
{
    [Fact]
    public void Query_WithoutSibling_Throws()
    {
        var query = CreateQuery().ToInjectable(typeof(Functions)).Select(m => Functions.VelocityWithoutSibling(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutSibling: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithConvention_Injects()
    {
        var query = CreateQuery().ToInjectable(typeof(Functions)).Select(m => Functions.VelocityWithConvention(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithTypeAndMethodMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithTypeAndMethodMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithTypeMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithTypeMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithMethodMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithMethodMetadata(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithNullLambda_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithNullLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Lambda factory for VelocityWithNullLambda returns null.", error.Message);
    }

    [Fact]
    public void Query_WithoutLambda_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutLambda(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutLambda: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutExpression_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutExpression(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutExpression: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutSignature_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutSignature(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutSignature: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureType_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutMatchingSignatureType(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutMatchingSignatureType: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureSize_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutMatchingSignatureSize(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutMatchingSignatureSize: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithGenericArgument_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithGenericArgument(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithoutMatchingGenericArgument_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithoutMatchingGenericArgument(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Static+Functions.VelocityWithoutMatchingGenericArgument: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithPrivateSibling_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithPrivateSibling(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithCachedExpression_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => Functions.VelocityWithCachedExpression(m));

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
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

    private static class Functions
    {
        public static double VelocityWithoutSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static double VelocityWithConvention(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityWithConvention()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityWithMetadata()
            => v => v.Distance / v.Time;

        [InjectLambda(typeof(OtherFunctions), nameof(OtherFunctions.Narf))]
        public static double VelocityWithTypeAndMethodMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda(typeof(OtherFunctions))]
        public static double VelocityWithTypeMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        [InjectLambda(nameof(Narf))]
        public static double VelocityWithMethodMetadata(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> Narf()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithNullLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityWithNullLambda()
            => null!;

        [InjectLambda]
        public static double VelocityWithoutLambda(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static IEnumerable<Func<Model, double>> VelocityWithoutLambda()
            => [v => v.Distance / v.Time];

        [InjectLambda]
        public static double VelocityWithoutExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Func<Model, double> VelocityWithoutExpression()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithoutSignature(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static LambdaExpression VelocityWithoutSignature()
            => (Expression<Func<Model, double>>)(v => v.Distance / v.Time);

        [InjectLambda]
        public static double VelocityWithoutMatchingSignatureType(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, float>> VelocityWithoutMatchingSignatureType()
            => v => (float)(v.Distance / v.Time);

        [InjectLambda]
        public static double VelocityWithoutMatchingSignatureSize(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<double, double, double>> VelocityWithoutMatchingSignatureSize()
            => (d, t) => d / t;

        [InjectLambda]
        public static double VelocityWithGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<TModel, double>> VelocityWithGenericArgument<TModel>()
            where TModel : IModel
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithoutMatchingGenericArgument<TModel>(TModel value)
            where TModel : IModel
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityWithoutMatchingGenericArgument()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithPrivateSibling(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        private static Expression<Func<Model, double>> VelocityWithPrivateSibling()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public static double VelocityWithCachedExpression(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        private static CachedExpression<Func<Model, double>> VelocityWithCachedExpressionExpr { get; }
            = CachedExpression.From<Func<Model, double>>(v => v.Distance / v.Time);
    }

    private static class OtherFunctions
    {
        public static Expression<Func<Model, double>> Narf()
            => v => v.Distance / v.Time;

        public static Expression<Func<Model, double>> VelocityWithTypeMetadata()
            => v => v.Distance / v.Time;
    }
}
