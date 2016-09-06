using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class NotTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>> p = null;

            Assert.Throws<ArgumentNullException>(() => p.Not());
        }

        [Fact]
        public void ShouldNegatePredicate()
        {
            Expression<Func<IDummy, bool>> p = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(p.Not()).Count();

            Assert.Equal(4, r);
            Assert.Equal(8, s);
        }
    }
}
