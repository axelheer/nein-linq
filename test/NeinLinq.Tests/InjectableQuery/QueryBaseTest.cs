using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryBaseTest
    {
        private readonly FunctionsBase functions = new ConcreteFunctions(1);

        private readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithoutSibling(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithoutSibling: no matching parameterless member found.", error.Message);
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
        public void ShouldFailWithStupidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithStupidSiblingResult(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithStupidSiblingResult: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithInvalidSiblingResult(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithInvalidSiblingResult: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithStupidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithStupidSiblingSignature(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithStupidSiblingSignature: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithInvalidSiblingSignature(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithInvalidSiblingSignature: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithGenericArguments()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithGenericArguments(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }

        [Fact]
        public void ShouldFailWithInvalidGenericArguments()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithInvalidGenericArguments(d);

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.ConcreteFunctions.VelocityWithInvalidGenericArguments: no matching parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithNonPublicSibling()
        {
            var query = from d in data.ToInjectable(typeof(FunctionsBase))
                        select functions.VelocityWithNonPublicSibling(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }
    }
}
