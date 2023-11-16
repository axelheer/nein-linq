using Xunit;

namespace NeinLinq.Tests;

public class InjectLambdaQueryTest_Property
{
    [Fact]
    public void Query_OrdinaryProperty_Ignores()
    {
        var query = CreateQuery().ToInjectable(typeof(Model)).Select(m => m.Name);

        var result = query.ToList();

        Assert.Equal(["Asdf", "Narf", "Qwer"], result);
    }

    [Fact]
    public void Query_ReadonlyProperty_Ignores()
    {
        var query = CreateQuery().Select(m => m.Velocity);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithoutSibling_Throws()
    {
        var query = CreateQuery().ToInjectable(typeof(Model)).Select(m => m.VelocityWithoutSibling);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.VelocityWithoutSiblingExpr: no matching parameterless member found.", error.Message);
    }

    [Fact]
    public void Query_WithConvention_Injects()
    {
        var query = CreateQuery().ToInjectable(typeof(Model)).Select(m => m.VelocityWithConvention);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithMetadata);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithTypeMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithTypeMetadata);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithWithTypeMetadataOnGetter_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithTypeMetadataOnGetter);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithMethodMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithMethodMetadata);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithMethodMetadataOnGetter_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithMethodMetadataOnGetter);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_VelocityWithProperty_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithProperty);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithPropertyAndTypeMetadata_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithPropertyAndTypeMetadata);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithNullLambda_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithNullLambda);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Lambda factory for VelocityWithNullLambda returns null.", error.Message);
    }

    [Fact]
    public void Query_WithoutLambda_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithoutLambda);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.VelocityWithoutLambdaExpr: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutExpression_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithoutExpression);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.VelocityWithoutExpressionExpr: returns no lambda expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutSignature_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithoutSignature);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.VelocityWithoutSignatureExpr: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureSize_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.WithoutMatchingSignatureSize);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.WithoutMatchingSignatureSizeExpr: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithoutMatchingSignatureType_Throws()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.WithoutMatchingSignatureType);

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest_Property+Model.WithoutMatchingSignatureTypeExpr: returns non-matching expression.", error.Message);
    }

    [Fact]
    public void Query_WithPrivateSibling_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithPrivateSibling);

        var result = query.ToList();

        Assert.Equal([200.0, .0, .125], result);
    }

    [Fact]
    public void Query_WithCachedExpression_Injects()
    {
        var query = CreateQuery().ToInjectable().Select(m => m.VelocityWithCachedExpression);

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

    private class Model
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double Distance { get; set; }

        public double Time { get; set; }

        public double Velocity
            => Distance / Time;

        public double VelocityWithoutSibling { get; }

        public double VelocityWithConvention { get; }

        public static Expression<Func<Model, double>> VelocityWithConventionExpr
            => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithMetadata { get; }

        public static Expression<Func<Model, double>> VelocityWithMetadataExpr
            => v => v.Distance / v.Time;

        [InjectLambda(typeof(ModelExtensions))]
        public double VelocityWithTypeMetadata { get; }

        public double VelocityWithTypeMetadataOnGetter
        {
            [InjectLambda(typeof(ModelExtensions), nameof(ModelExtensions.VelocityWithTypeMetadata))]
            get => throw new NotSupportedException();
        }

        [InjectLambda(nameof(Narf))]
        public double VelocityWithMethodMetadata { get; }

        public double VelocityWithMethodMetadataOnGetter
        {
            [InjectLambda(nameof(Narf))]
            get => 0;
        }

        public static Expression<Func<Model, double>> Narf()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithProperty { get; }

        public static Expression<Func<Model, double>> VelocityWithPropertyExpr
            => v => v.Distance / v.Time;

        [InjectLambda(typeof(ModelExtensions))]
        public double VelocityWithPropertyAndTypeMetadata { get; }

        [InjectLambda]
        public double VelocityWithNullLambda { get; }

        public static Expression<Func<Model, double>> VelocityWithNullLambdaExpr
            => null!;

        [InjectLambda]
        public double VelocityWithoutLambda { get; }

        public static Lazy<Func<Model, double>> VelocityWithoutLambdaExpr
            => new(() => v => v.Distance / v.Time);

        [InjectLambda]
        public double VelocityWithoutExpression { get; }

        public static Func<Model, double> VelocityWithoutExpressionExpr
            => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithoutSignature { get; }

        public static LambdaExpression VelocityWithoutSignatureExpr
            => (Expression<Func<Model, double>>)(v => v.Distance / v.Time);

        [InjectLambda]
        public double WithoutMatchingSignatureType { get; }

        public static Expression<Func<Model, float>> WithoutMatchingSignatureTypeExpr
            => v => (float)(v.Distance / v.Time);

        [InjectLambda]
        public double WithoutMatchingSignatureSize { get; }

        public static Expression<Func<double, double, double>> WithoutMatchingSignatureSizeExpr
            => (d, t) => d / t;

        [InjectLambda]
        public double VelocityWithPrivateSibling { get; }

        private static Expression<Func<Model, double>> VelocityWithPrivateSiblingExpr
            => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithCachedExpression { get; }

        private static CachedExpression<Func<Model, double>> VelocityWithCachedExpressionExpr
            => new(v => v.Distance / v.Time);
    }

    private static class ModelExtensions
    {
        public static Expression<Func<Model, double>> VelocityWithTypeMetadata()
            => v => v.Distance / v.Time;

        public static Expression<Func<Model, double>> VelocityWithPropertyAndTypeMetadata
            => v => v.Distance / v.Time;
    }
}
