using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQueryData
{
    public class Dummy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Distance { get; set; }

        public double Time { get; set; }

        [InjectLambda(nameof(InjectVelocityInternal))]
        public double VelocityInternal { get; }

        public double VelocityInternalWithGetter
        {
            [InjectLambda(nameof(InjectVelocityInternal))]
            get { throw new NotSupportedException(); }
        }

        public static Expression<Func<Dummy, double>> InjectVelocityInternal()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(DummyExtensions))]
        public double VelocityExternal { get; }

        public double VelocityExternalWithGetter
        {
            [InjectLambda(typeof(DummyExtensions), nameof(DummyExtensions.VelocityExternal))]
            get { throw new NotSupportedException(); }
        }

        public double VelocityWithConvention => 0;

        public static Expression<Func<Dummy, double>> VelocityWithConventionExpr => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithMetadata => 0;

        public static Expression<Func<Dummy, double>> VelocityWithMetadataExpr => v => v.Distance / v.Time;
    }
}
