using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ToTranslationTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, bool>> p = _ => false;

            Assert.Throws<ArgumentNullException>(() => p.Translate().To(default(Expression<Func<ParentDummy, Func<ChildDummy, bool>, bool>>)));
        }

        [Fact]
        public void ShouldSubstitute()
        {
            Expression<Func<ChildDummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<ChildDummy>().Where(p).Count();
            var s = data.OfType<ParentDummy>().Where(p.Translate().To<ParentDummy>((b, q) => b.Children.Any(q))).Count();

            Assert.Equal(1, r);
            Assert.Equal(2, s);
        }
    }
}
