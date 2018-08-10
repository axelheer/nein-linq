using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.PredicateTranslator;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ToSubtypeTest
    {
        private readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<Dummy>().Where(p).Count();
            var s = data.OfType<SpecialDummy>().Where(p.Translate().To<SpecialDummy>()).Count();

            Assert.Equal(2, r);
            Assert.Equal(1, s);
        }
    }
}
