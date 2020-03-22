using System;
using System.Linq.Expressions;

namespace NeinLinq.Fakes.InjectableQuery
{
    public static class DummyExtensions
    {
        public static Expression<Func<Dummy, double>> VelocityExternal()
            => v => v.Distance / v.Time;

        public static Expression<Func<Dummy, double>> VelocityExternalProperty
            => v => v.Distance / v.Time;
    }
}
