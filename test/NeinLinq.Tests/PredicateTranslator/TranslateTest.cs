using NeinLinq.Fakes.PredicateTranslator;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class TranslateTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<Dummy, bool>> p = null;

            Assert.Throws<ArgumentNullException>(() => p.Translate());
        }
    }
}
