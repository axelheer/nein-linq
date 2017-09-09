using System;
using System.Linq.Expressions;

namespace NeinLinq.Fakes.InjectableQuery
{
    class GenericOtherFunctions
    {
        public static Expression<Func<TDummy, double>> Narf<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        public static Expression<Func<TDummy, double>> VelocityWithTypeMetadata<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }
    }
}
