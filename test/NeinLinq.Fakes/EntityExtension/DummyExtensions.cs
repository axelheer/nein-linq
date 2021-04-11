using System;
using System.Linq.Expressions;

#pragma warning disable CA1801
#pragma warning disable CA1822

namespace NeinLinq.Fakes.EntityExtension
{
    public static class DummyExtensions
    {
        [InjectLambda]
        public static bool IsNarf(this Dummy dummy)
            => throw new InvalidOperationException();

        public static Expression<Func<Dummy, bool>> IsNarf()
            => d => d.Name == "Narf";
    }
}
