using NeinLinq.Tests.SelectorTranslatorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class SelectorTranslatorTest
    {
        readonly IQueryable<IDummy> data;

        public SelectorTranslatorTest()
        {
            var d = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" }
            };
            var s = new[]
            {
                new SuperDummy { Id = 4, Name = "Asdf", Description = "Asdf" },
                new SuperDummy { Id = 5, Name = "Narf", Description = "Narf" },
                new SuperDummy { Id = 6, Name = "Qwer", Description = "Qwer" }
            };
            var p = new[]
            {
                new ParentDummy { Id = 7, Name = "Asdf" },
                new ParentDummy { Id = 8, Name = "Narf" },
                new ParentDummy { Id = 9, Name = "Qwer" }
            };
            var c = new[]
            {
                new ChildDummy { Id = 10, Name = "Asdf", Parent = p[1] },
                new ChildDummy { Id = 11, Name = "Narf", Parent = p[2] },
                new ChildDummy { Id = 12, Name = "Qwer", Parent = p[0] }
            };
            p[0].Children = new[] { c[0], c[1] };
            p[1].Children = new[] { c[1], c[2] };
            p[2].Children = new[] { c[2], c[0] };

            data = d.Concat<IDummy>(s).Concat(p).Concat(c).AsQueryable();
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new SelectorTranslation<Dummy, DummyView>(null));
        }

        [Fact]
        public void ApplyShouldHandleInvalidArguments()
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
        public void ApplyShouldMergeSelectors()
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

        [Fact]
        public void SourceSubtypeShouldSubstitute()
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
        public void ResultSubtypeShouldHandleInvalidArguments()
        {
            Expression<Func<SuperDummy, DummyView>> s = _ => null;

            Assert.Throws<NotSupportedException>(() => s.Translate().Result<SuperDummyView>());

            s = _ => new DummyView(1) { Name = "Narf" };

            Assert.Throws<NotSupportedException>(() => s.Translate().Result<SuperDummyView>());
        }

        [Fact]
        public void ResultSubtypeShouldSubstitute()
        {
            Expression<Func<SuperDummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Result<SuperDummyView>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Null(v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Null(v.Description); });
        }

        [Fact]
        public void CrossSubtypeApplyShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = d => new SuperDummyView { Description = d.Description };

            var select = s.Translate().Cross<SuperDummy>().Apply(t);
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal("Asdf", v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal("Narf", v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal("Qwer", v.Description); });
        }

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = d => new SuperDummyView { Description = d.Description };

            var select = s.Translate().To(t);
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal("Asdf", v.Description); },
                v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal("Narf", v.Description); },
                v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal("Qwer", v.Description); });
        }

        [Fact]
        public void SourcePathShouldShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = _ => new ParentDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ChildDummy, ParentDummy>>)));
        }

        [Fact]
        public void SourcePathShouldSubstitute()
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
        public void ResultPathShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ParentDummyView>> s = _ => new ParentDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Result(default(Expression<Func<ChildDummyView, ParentDummyView>>)));
            Assert.Throws<NotSupportedException>(() => s.Translate().Result<ChildDummyView>(d => null));
        }

        [Fact]
        public void ResultPathShouldSubstitute()
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
        public void CrossPathApplyShouldSubstitute()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ChildDummy, ChildDummyView>> t = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<ChildDummy>(d => d.Parent).Apply(d => d.Parent, t);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
                v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
                v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
        }

        [Fact]
        public void ToPathShouldSubstitute()
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
        public void SourceTranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = _ => new ChildDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ParentDummy, Func<ChildDummy, ChildDummyView>, ChildDummyView>>)));
        }

        [Fact]
        public void SourceTranslationShouldSubstitute()
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
        public void SourceTranslationCollectionShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = _ => new ChildDummyView();

            Assert.Throws<ArgumentNullException>(() => s.Translate().Source(default(Expression<Func<ParentDummy, Func<ChildDummy, ChildDummyView>, IEnumerable<ChildDummyView>>>)));
        }

        [Fact]
        public void SourceTranslationCollectionShouldSubstitute()
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

        [Fact]
        public void ResultTranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, IEnumerable<ChildDummyView>>> s = _ => new ChildDummyView[0];

            Assert.Throws<ArgumentNullException>(() => s.Translate().Result(default(Expression<Func<ParentDummy, Func<ParentDummy, IEnumerable<ChildDummyView>>, ParentDummyView>>)));
        }

        [Fact]
        public void ResultTranslationShouldSubstitute()
        {
            Expression<Func<ParentDummy, IEnumerable<ChildDummyView>>> s = d => d.Children.Select(e => new ChildDummyView { Id = e.Id, Name = e.Name });

            var select = s.Translate().Result((d, v) => new ParentDummyView { FirstChild = v(d).First() });
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }

        [Fact]
        public void CrossTranslationApplyShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ParentDummy, ParentDummyView>> t = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<ParentDummy>((d, v) => d.Children.Select(v).First()).Apply((d, v) => new ParentDummyView { FirstChild = v(d) }, t);
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }

        [Fact]
        public void CrossTranslationCollectionApplyShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ParentDummy, ParentDummyView>> t = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<ParentDummy>((d, v) => d.Children.Select(v)).Apply((d, v) => new ParentDummyView { FirstChild = v(d).First() }, t);
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.Collection(result,
                v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
                v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
                v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
        }

        [Fact]
        public void ToTranslationShouldSubstitute()
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
    }
}
