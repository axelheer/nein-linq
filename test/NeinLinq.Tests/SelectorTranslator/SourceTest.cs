using NeinLinq.Fakes.SelectorTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class SourceTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void SubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<SuperDummy>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); });
        }

        [Fact]
        public void PathShouldShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = _ => new ParentDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ChildDummy, ParentDummy>>)));
        }

        [Fact]
        public void PathShouldSubstitute()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<ChildDummy>(d => d.Parent);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); },
                v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); });
        }

        [Fact]
        public void TranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = _ => new ChildDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ParentDummy, Func<ChildDummy, ChildDummyView>, ChildDummyView>>)));
        }

        [Fact]
        public void TranslationShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<ParentDummy>((d, v) => d.Children.Select(v).First());
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); },
                v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); },
                v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); });
        }

        [Fact]
        public void TranslationCollectionShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = _ => new ChildDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ParentDummy, Func<ChildDummy, ChildDummyView>, IEnumerable<ChildDummyView>>>)));
        }

        [Fact]
        public void TranslationCollectionShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<ParentDummy>((d, v) => d.Children.Select(v));
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => Assert.Collection(v,
                    w => { Assert.Equal(10, w.Id); Assert.Equal("Asdf", w.Name); },
                    w => { Assert.Equal(11, w.Id); Assert.Equal("Narf", w.Name); }),
                v => Assert.Collection(v,
                    w => { Assert.Equal(11, w.Id); Assert.Equal("Narf", w.Name); },
                    w => { Assert.Equal(12, w.Id); Assert.Equal("Qwer", w.Name); }),
                v => Assert.Collection(v,
                    w => { Assert.Equal(12, w.Id); Assert.Equal("Qwer", w.Name); },
                    w => { Assert.Equal(10, w.Id); Assert.Equal("Asdf", w.Name); }));
        }
    }
}
