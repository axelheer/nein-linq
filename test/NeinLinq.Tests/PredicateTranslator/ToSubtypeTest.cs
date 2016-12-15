using NeinLinq.Fakes.PredicateTranslator;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ToSubtypeTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<Dummy>().Where(p).Count();
            var s = data.OfType<SuperDummy>().Where(p.Translate().To<SuperDummy>()).Count();

            Assert.Equal(2, r);
            Assert.Equal(1, s);
        }
    }
}
