using NeinLinq.Fakes.SelectorTranslator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class ResultTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void SubtypeShouldHandleInvalidArguments()
        {
            Expression<Func<SpecialDummy, DummyView>> s = _ => null;

            Assert.Throws<NotSupportedException>(() => s.Translate().Result<SpecialDummyView>());

            s = _ => new DummyView(1) { Name = "Narf" };

            Assert.Throws<NotSupportedException>(() => s.Translate().Result<SpecialDummyView>());
        }

        [Fact]
        public void SubtypeShouldSubstitute()
        {
            Expression<Func<SpecialDummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Result<SpecialDummyView>();
            var result = data.OfType<SpecialDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Null(v.Description); });
        }

        [Fact]
        public void PathShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ParentDummyView>> s = _ => new ParentDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Result(default(Expression<Func<ChildDummyView, ParentDummyView>>)));
            Assert.Throws<NotSupportedException>(() => s.Translate().Result<ChildDummyView>(d => null));
        }

        [Fact]
        public void PathShouldSubstitute()
        {
            Expression<Func<ChildDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Parent.Id, Name = d.Parent.Name };

            var select = s.Translate().Result<ChildDummyView>(d => d.Parent);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
                v => { Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
                v => { Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
        }

        [Fact]
        public void TranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, IEnumerable<ChildDummyView>>> s = _ => new ChildDummyView[0];

            Assert.Throws<ArgumentNullException>(() => s.Translate().Result(default(Expression<Func<ParentDummy, Func<ParentDummy, IEnumerable<ChildDummyView>>, ParentDummyView>>)));
        }

        [Fact]
        public void TranslationShouldSubstitute()
        {
            Expression<Func<ParentDummy, IEnumerable<ChildDummyView>>> s = d => d.Children.Select(e => new ChildDummyView { Id = e.Id, Name = e.Name });

            var select = s.Translate().Result((d, v) => new ParentDummyView { FirstChild = v(d).First() });
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }
    }
}
