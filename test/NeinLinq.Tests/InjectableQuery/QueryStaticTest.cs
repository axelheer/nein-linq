using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryStaticTest
    {
        private readonly IQueryable<Dummy> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithoutSibling();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithoutSibling: no matching parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithConvention();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithTypeAndMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeAndMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithTypeMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldFailWithNull()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithNull();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Lambda factory for VelocityWithNull returns null.", error.Message);
        }

        [Fact]
        public void ShouldFailWithStupidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithStupidSiblingResult();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithStupidSiblingResult: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingResult();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithInvalidSiblingResult: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithWeakSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithWeakSiblingResult();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithWeakSiblingResult: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithStupidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithStupidSiblingSignature();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithStupidSiblingSignature: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingSignature();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithInvalidSiblingSignature: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithGenericArguments()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithGenericArguments();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldFailWithInvalidGenericArguments()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidGenericArguments();

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Functions.VelocityWithInvalidGenericArguments: no matching parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithNonPublicSibling()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithNonPublicSibling();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithCachedExpression()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithCachedExpression();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithCachedDelegate()
        {
            var query = from d in data
                        select d.VelocityWithCachedExpression();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }
    }
}
