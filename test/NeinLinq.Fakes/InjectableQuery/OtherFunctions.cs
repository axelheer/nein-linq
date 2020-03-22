using System;
using System.Linq.Expressions;

namespace NeinLinq.Fakes.InjectableQuery
{
    public static class OtherFunctions
    {
        public static Expression<Func<Dummy, double>> Narf()
            => v => v.Distance / v.Time;

        public static Expression<Func<Dummy, double>> VelocityWithTypeMetadata()
            => v => v.Distance / v.Time;
    }
}
