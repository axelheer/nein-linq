using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class InjectPropertyTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

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
    }
}
