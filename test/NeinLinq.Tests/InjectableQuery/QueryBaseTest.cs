using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryBaseTest
    {
        readonly FunctionsBase functions = new ConcreteFunctions(1);

        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithoutSibling(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithoutSibling: no parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
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
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithInvalidSiblingResult(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithInvalidSiblingResult: method returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithInvalidSiblingSignature(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithInvalidSiblingSignature: method returns non-matching expression.", error.Message);
        }
    }
}
