using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryMixedTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void StaticToInstanceShouldFail()
        {
            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select MixedFunctions.VelocityStaticToInstance(d);

            Assert.Throws<InvalidOperationException>(() => query.ToList());
        }

        [Fact]
        public void InstanceToStaticShouldFail()
        {
            var functions = new MixedFunctions(1);

            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select functions.VelocityInstanceToStatic(d);

            Assert.Throws<InvalidOperationException>(() => query.ToList());
        }
    }
}
