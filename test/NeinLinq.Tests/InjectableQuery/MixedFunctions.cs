using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQuery
{
    public class MixedFunctions
    {
        readonly int digits;

        public MixedFunctions(int digits)
        {
            this.digits = digits;
        }

        public double VelocityInstanceToStatic(Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityInstanceToStatic()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityStaticToInstance(Dummy value)
        {
            throw new NotSupportedException();
        }

        public Expression<Func<Dummy, double>> VelocityStaticToInstance()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }
    }
}
