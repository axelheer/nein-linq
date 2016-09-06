using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQuery
{
    public static class OtherFunctions
    {
        public static Expression<Func<Dummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        public static Expression<Func<Dummy, double>> VelocityWithTypeMetadata()
        {
            return v => v.Distance / v.Time;
        }
    }
}
