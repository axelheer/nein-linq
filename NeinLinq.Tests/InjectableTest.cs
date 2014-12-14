using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableTest
    {
        private readonly IQueryable<InjectableDummy> data;

        public InjectableTest()
        {
            data = new[]
            {
                new InjectableDummy { Id = 1, Name = "Asdf", Distance = 66, Time = .33 },
                new InjectableDummy { Id = 2, Name = "Narf", Distance = 0, Time = 3.14 },
                new InjectableDummy { Id = 3, Name = "Qwer", Distance = 8, Time = .125 }
            }
            .AsQueryable();
        }

        [Fact]
        public void InjectShouldFailWithoutSibling()
        {
            var query = from d in data.ToInjectable(typeof(InjectableFunctions))
                        select d.VelocityWithoutSibling();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectShouldSucceedWithConvention()
        {
            var query = from d in data.ToInjectable(typeof(InjectableFunctions))
                        select d.VelocityWithConvention();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, 0.0, 64.0 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, 0.0, 64.0 }, result);
        }

        [Fact]
        public void InjectShouldSucceedWithAdvancedMetadata()
        {
            var query = from d in data.ToInjectable()
                        select d.VelocityWithAdvancedMetadata();

            var result = query.ToList();

            Assert.Equal(new[] { 200.0, 0.0, 64.0 }, result);
        }

        [Fact]
        public void InjectShouldFailWithInvalidSiblingResult()
        {
            var query = from d in data.ToInjectable(typeof(InjectableFunctions))
                        select d.VelocityWithInvalidSiblingResult();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }

        [Fact]
        public void InjectShouldFailWithInvalidSiblingSignature()
        {
            var query = from d in data.ToInjectable(typeof(InjectableFunctions))
                        select d.VelocityWithInvalidSiblingSignature();

            Assert.Throws<InvalidOperationException>(() =>
                query.ToList());
        }
    }
}
