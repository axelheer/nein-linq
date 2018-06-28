using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Queryable;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryPropertyTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldIgnoreOrdinaryProperty()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.Name;

            var result = query.ToList();

            Assert.Equal(new[] { "Asdf", "Narf", "Qwer" }, result);
        }

        [Fact]
        public void ShouldAcceptOrdinaryProperty()
        {
            var query = from d in data.ToInjectable()
                        select d.Velocity;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithoutSibling;

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Dummy.VelocityWithoutSiblingExpr: no matching parameterless member found.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithConvention;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithInternalMethod()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityInternal;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithInternalMethodWithGetter()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityInternalWithGetter;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithExternalMethod()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityExternal;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithExternalMethodWithGetter()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityExternalWithGetter;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithInternalProperty()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityInternalProperty;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldSucceedWithExternalProperty()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityExternalProperty;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void ShouldFailWithStupidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithStupidSiblingResult;

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Dummy.VelocityWithStupidSiblingResultExpr: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithInvalidSiblingResult;

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Dummy.VelocityWithInvalidSiblingResultExpr: returns no lambda expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithStupidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithStupidSiblingSignature;

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Dummy.VelocityWithStupidSiblingSignatureExpr: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithInvalidSiblingSignature;

            var error = Assert.Throws<InvalidOperationException>(() => query.ToList());

            Assert.Equal("Unable to retrieve lambda expression from NeinLinq.Fakes.InjectableQuery.Dummy.VelocityWithInvalidSiblingSignatureExpr: returns non-matching expression.", error.Message);
        }

        [Fact]
        public void ShouldSucceedWithNonPublicSibling()
        {
            var query = from d in data.ToInjectable(typeof(Dummy))
                        select d.VelocityWithNonPublicSibling;

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }
    }
}
