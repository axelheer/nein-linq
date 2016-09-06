using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ToPathTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, bool>> p = _ => false;

            Assert.Throws<ArgumentNullException>(() => p.Translate().To(default(Expression<Func<ChildDummy, ParentDummy>>)));
        }

        [Fact]
        public void ShouldSubstitute()
        {
            Expression<Func<ParentDummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<ParentDummy>().Where(p).Count();
            var s = data.OfType<ChildDummy>().Where(p.Translate().To<ChildDummy>(c => c.Parent)).Count();

            Assert.Equal(1, r);
            Assert.Equal(1, s);
        }
    }
}
