using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQuery
{
    public static class DummyExtensions
    {
        public static Expression<Func<Dummy, double>> VelocityExternal()
        {
            return v => v.Distance / v.Time;
        }
    }
}
