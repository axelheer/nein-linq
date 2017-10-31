using NeinLinq.Fakes.SelectorTranslator;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class ToTest
    {
        readonly IQueryable<IDummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void SubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SpecialDummy, SpecialDummyView>> t = d => new SpecialDummyView { Description = d.Description };

            var select = s.Translate().To(t);
            var result = data.OfType<SpecialDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal("Asdf", v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal("Narf", v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal("Qwer", v.Description); });
        }

        [Fact]
        public void SubtypeShouldSubstituteWithoutValue()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().To<SpecialDummy, SpecialDummyView>();
            var result = data.OfType<SpecialDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Null(v.Description); });
        }

        [Fact]
        public void PathShouldSubstitute()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ChildDummy, ChildDummyView>> t = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().To(d => d.Parent, d => d.Parent, t);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
                v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
                v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
        }

        [Fact]
        public void PathShouldSubstituteWithoutValue()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().To<ChildDummy, ChildDummyView>(d => d.Parent, d => d.Parent);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
        }

        [Fact]
        public void TranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            Assert.Throws<ArgumentNullException>(() => s.Translate().To(default(Expression<Func<ParentDummy, Func<ChildDummy, ChildDummyView>, ParentDummyView>>)));
        }

        [Fact]
        public void TranslationShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ParentDummy, ParentDummyView>> t = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().To((d, v) => new ParentDummyView { FirstChild = d.Children.Select(v).First() }, t);
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }

        [Fact]
        public void TranslationShouldSubstituteWithoutValue()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().To<ParentDummy, ParentDummyView>((d, v) => new ParentDummyView { FirstChild = d.Children.Select(v).First() });
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }
    }
}
