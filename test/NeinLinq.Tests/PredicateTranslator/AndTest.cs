using NeinLinq.Fakes.PredicateTranslator;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class AndTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>> p = _ => false;
            Expression<Func<IDummy, bool>> q = null;

            Assert.Throws<ArgumentNullException>(() => p.And(q));
            Assert.Throws<ArgumentNullException>(() => q.And(p));
        }

        [Fact]
        public void ShouldCombinePredicates()
        {
            Expression<Func<IDummy, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<IDummy, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.And(q)).Count();

            Assert.Equal(6, r);
            Assert.Equal(4, s);
            Assert.Equal(2, t);
        }
    }
}
