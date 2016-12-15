using NeinLinq.Fakes.SelectorTranslator;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class ApplyTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<Dummy, DummyView>> s = _ => new DummyView { Id = 1 };
            Expression<Func<Dummy, DummyView>> t = null;

            Assert.Throws<ArgumentNullException>(() => s.Apply(t));
            Assert.Throws<ArgumentNullException>(() => t.Apply(s));

            t = _ => null;

            Assert.Throws<NotSupportedException>(() => s.Apply(t));
            Assert.Throws<NotSupportedException>(() => t.Apply(s));

            t = _ => new DummyView(1) { Name = "Narf" };

            Assert.Throws<NotSupportedException>(() => s.Apply(t));
            Assert.Throws<NotSupportedException>(() => t.Apply(s));
        }

        [Fact]
        public void ShouldMergeSelectors()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id };
            Expression<Func<Dummy, DummyView>> t = d => new DummyView { Name = d.Name };

            var select = s.Apply(t);
            var result = data.OfType<Dummy>().Except(data.OfType<SuperDummy>()).Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(1, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(2, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(3, v.Id); Assert.Equal("Qwer", v.Name); });
        }
    }
}
