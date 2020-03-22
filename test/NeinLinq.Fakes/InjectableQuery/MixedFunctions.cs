using System;
using System.Linq.Expressions;

#pragma warning disable CA1801
#pragma warning disable CA1822

namespace NeinLinq.Fakes.InjectableQuery
{
    public class MixedFunctions
    {
        private readonly int digits;

        public MixedFunctions(int digits)
        {
            this.digits = digits;
        }

        public double VelocityInstanceToStatic(Dummy value)
            => throw new NotSupportedException();

        public static Expression<Func<Dummy, double>> VelocityInstanceToStatic()
            => v => v.Distance / v.Time;

        public static double VelocityStaticToInstance(Dummy value)
            => throw new NotSupportedException();

        public Expression<Func<Dummy, double>> VelocityStaticToInstance()
            => v => Math.Round(v.Distance / v.Time, digits);
    }
}
