using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.PredicateTranslator;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class NotTest
    {
        private readonly IQueryable<IDummy> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>>? p = null;

            _ = Assert.Throws<ArgumentNullException>(() => p!.Not());
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
