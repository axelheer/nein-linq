using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.SelectorTranslator;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class ApplyTest
    {
        private readonly IQueryable<IDummy> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<Dummy, DummyView>> s = _ => new DummyView { Id = 1 };
            Expression<Func<Dummy, DummyView?>>? t = null;

            _ = Assert.Throws<ArgumentNullException>(() => s.Apply(t!));
            _ = Assert.Throws<ArgumentNullException>(() => t!.Apply(s));

            t = _ => null;

            _ = Assert.Throws<NotSupportedException>(() => s.Apply(t!));
            _ = Assert.Throws<NotSupportedException>(() => t!.Apply(s));

            t = _ => new DummyView(1) { Name = "Narf" };

            _ = Assert.Throws<NotSupportedException>(() => s.Apply(t!));
            _ = Assert.Throws<NotSupportedException>(() => t!.Apply(s));
        }

        [Fact]
        public void ShouldMergeSelectors()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id };
            Expression<Func<Dummy, DummyView>> t = d => new DummyView { Name = d.Name };

            var select = s.Apply(t);
            var result = data.OfType<Dummy>().Except(data.OfType<SpecialDummy>()).Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(1, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(2, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(3, v.Id); Assert.Equal("Qwer", v.Name); });
        }

        [Fact]
        public void ShouldNotThrowOnEmptyNewExpression()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView();
            Expression<Func<Dummy, DummyView>> t = d => new DummyView();

            var select = s.Apply(t);
            var result = data.OfType<Dummy>().Except(data.OfType<SpecialDummy>()).Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); });
        }

        [Fact]
        public void ShouldNotLoseBindingsOnMixedExpressions()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView();
            Expression<Func<Dummy, DummyView>> t = d => new DummyView
            {
                Id = d.Id + 5,
                Name = d.Name
            };

            var select = s.Apply(t);
            var result = data.OfType<Dummy>().Except(data.OfType<SpecialDummy>()).Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(6, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(7, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(8, v.Id); Assert.Equal("Qwer", v.Name); }
            );
        }

        [Fact]
        public void ShouldNotLoseBindingsOnMixedExpressionsReversed()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView();
            Expression<Func<Dummy, DummyView>> t = d => new DummyView
            {
                Id = d.Id + 5,
                Name = d.Name
            };

            var select = t.Apply(s);
            var result = data.OfType<Dummy>().Except(data.OfType<SpecialDummy>()).Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(6, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(7, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(8, v.Id); Assert.Equal("Qwer", v.Name); }
            );
        }
    }
}
