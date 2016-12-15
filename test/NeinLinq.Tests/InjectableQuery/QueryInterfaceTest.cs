using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryInterfaceTest
    {
        readonly IFunctions functions = new ConcreteFunctions(1);

        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithoutSibling(d);

            Assert.Throws<InvalidOperationException>(() => query.ToList());
        }

        [Fact]
        public void ShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithConvention(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select functions.VelocityWithMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select functions.VelocityWithMethodMetadata(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithInvalidSiblingResult(d);

            Assert.Throws<InvalidOperationException>(() => query.ToList());
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(IFunctions))
                        select functions.VelocityWithInvalidSiblingSignature(d);

            Assert.Throws<InvalidOperationException>(() => query.ToList());
        }
    }
}
