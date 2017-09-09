using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryGenericTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(GenericFunctions))
                        select d.VelocityWithoutSibling<Dummy>();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.GenericFunctions.VelocityWithoutSibling: no parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(GenericFunctions))
                        select d.VelocityWithConvention<Dummy>();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata<Dummy>();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithTypeAndMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeAndMethodMetadata<Dummy>();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithTypeMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeMetadata<Dummy>();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMethodMetadata<Dummy>();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(GenericFunctions))
                        select d.VelocityWithInvalidSiblingResult<Dummy>();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.GenericFunctions.VelocityWithInvalidSiblingResult: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(GenericFunctions))
                        select d.VelocityWithInvalidSiblingSignature<Dummy>();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.GenericFunctions.VelocityWithInvalidSiblingSignature: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidGenericSibling()
        {
            var query = from d in data.ToInjectable(typeof(GenericFunctions))
                        select d.VelocityWithInvalidGenericSibling<Dummy>();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.GenericFunctions.VelocityWithInvalidGenericSibling: generic implementation expected.", error.Message);
        }
    }
}
