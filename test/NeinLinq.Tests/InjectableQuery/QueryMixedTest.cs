using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryMixedTest
    {
        private readonly IQueryable<Dummy> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void StaticToInstanceShouldFail()
        {
            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select MixedFunctions.VelocityStaticToInstance(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.MixedFunctions.VelocityStaticToInstance: static implementation expected.", error.Message);
        }

        [Fact]
        public void InstanceToStaticShouldFail()
        {
            var functions = new MixedFunctions(1);

            var query = from d in data.ToInjectable(typeof(MixedFunctions))
                        select functions.VelocityInstanceToStatic(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.MixedFunctions.VelocityInstanceToStatic: non-static implementation expected.", error.Message);
        }
    }
}
