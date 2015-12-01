using NeinLinq.Tests.InjectableQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableQueryTest
    {
        readonly IQueryable<Dummy> data;

        public InjectableQueryTest()
        {
            data = new[]
            {
                new Dummy { Id = 1, Name = "Asdf", Distance = 66, Time = .33 },
                new Dummy { Id = 2, Name = "Narf", Distance = 0, Time = 3.14 },
                new Dummy { Id = 3, Name = "Qwer", Distance = 8, Time = 64 }
            }
            .AsQueryable();
        }

        [Fact]
        public void InjectShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithoutSibling();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithConvention();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithTypeAndMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeAndMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithTypeMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithTypeMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithMethodMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMethodMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .125 }, result);
        }

        [Fact]
        public void InjectShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingResult();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(Functions))
                        select d.VelocityWithInvalidSiblingSignature();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectShouldSucceedWithObject()
        {
            var functions = new ParameterizedFunctions(1);
            var query = from d in data.ToInjectable(typeof(ParameterizedFunctions))
                        select functions.Velocity(d);

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, .0, .1 }, result);
        }
    }
}
