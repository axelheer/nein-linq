using Xunit;

namespace NeinLinq.Tests;

public class InjectLambdaQueryTest
{
    [Fact]
    public void Query_StaticToInstance_Throws()
    {
        var query = CreateQuery().ToInjectable(typeof(MixedFunctions)).Select(m => MixedFunctions.VelocityStaticToInstance(m));

        var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

        Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Tests.InjectLambdaQueryTest+MixedFunctions.VelocityStaticToInstance: static implementation expected.", error.Message);
    }

    [Fact]
    public void Query_InstanceToStatic_DontThrow()
    {
        var functions = new MixedFunctions(1);

        var query = CreateQuery().ToInjectable(typeof(MixedFunctions)).Select(m => functions.VelocityInstanceToStatic(m));

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
    }

    private class MixedFunctions
    {
        private readonly int digits;

        public MixedFunctions(int digits)
        {
            this.digits = digits;
        }

        public double VelocityInstanceToStatic(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public static Expression<Func<Model, double>> VelocityInstanceToStatic()
            => v => v.Distance / v.Time;

        public static double VelocityStaticToInstance(Model value)
            => throw new NotSupportedException($"Unable to process {value.Name}.");

        public Expression<Func<Model, double>> VelocityStaticToInstance()
            => v => Math.Round(v.Distance / v.Time, digits);
    }
}
