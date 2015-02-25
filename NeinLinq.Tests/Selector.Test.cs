using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.Selector
{
    public class Test
    {
        private readonly IQueryable<IDummy> data;

        public Test()
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
            p[0].Childs = new[] { c[0], c[1] };
            p[1].Childs = new[] { c[1], c[2] };
            p[2].Childs = new[] { c[2], c[0] };

            data = d.Concat<IDummy>(s).Concat(p).Concat(c).AsQueryable();
        }

        [Fact]
        public void ApplyShouldMergeSelectors()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id };
            Expression<Func<Dummy, DummyView>> t = d => new DummyView { Name = d.Name };

            var select = s.Apply(t);
            var result = data.OfType<Dummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 1 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 2 && v.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Qwer"));
        }

        [Fact]
        public void SourceSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<SuperDummy>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Qwer"));
        }

        [Fact]
        public void ResultSubtypeShouldSubstitute()
        {
            Expression<Func<SuperDummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Result<SuperDummyView>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Asdf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Narf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Qwer" && v.Description == null));
        }

        [Fact]
        public void CrossSubtypeApplyShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = d => new SuperDummyView { Description = d.Description };

            var select = s.Translate().Cross<SuperDummy>().Apply(t);
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Asdf" && v.Description == "Asdf"));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Narf" && v.Description == "Narf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Qwer" && v.Description == "Qwer"));
        }

        [Fact]
        public void SourcePathShouldSubstitute()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<ChildDummy>(d => d.Parent);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 7 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 8 && v.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 9 && v.Name == "Qwer"));
        }

        [Fact]
        public void ResultPathShouldSubstitute()
        {
            Expression<Func<ChildDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Parent.Id, Name = d.Parent.Name };

            var select = s.Translate().Result<ChildDummyView>(d => d.Parent);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.True(result.Any(v => v.Parent.Id == 7 && v.Parent.Name == "Asdf"));
            Assert.True(result.Any(v => v.Parent.Id == 8 && v.Parent.Name == "Narf"));
            Assert.True(result.Any(v => v.Parent.Id == 9 && v.Parent.Name == "Qwer"));
        }

        [Fact]
        public void CrossPathApplyShouldSubstitute()
        {
            Expression<Func<ParentDummy, ParentDummyView>> s = d => new ParentDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ChildDummy, ChildDummyView>> t = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<ChildDummy>(d => d.Parent).Apply(d => d.Parent, t);
            var result = data.OfType<ChildDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 10 && v.Name == "Asdf" && v.Parent.Id == 8 && v.Parent.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 11 && v.Name == "Narf" && v.Parent.Id == 9 && v.Parent.Name == "Qwer"));
            Assert.True(result.Any(v => v.Id == 12 && v.Name == "Qwer" && v.Parent.Id == 7 && v.Parent.Name == "Asdf"));
        }

        [Fact]
        public void SourceTranslationShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<ParentDummy>((d, v) => d.Childs.Select(e => v(e)));
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.True(result.Any(v => v.Any(w => w.Id == 10 && w.Name == "Asdf")));
            Assert.True(result.Any(v => v.Any(w => w.Id == 11 && w.Name == "Narf")));
            Assert.True(result.Any(v => v.Any(w => w.Id == 12 && w.Name == "Qwer")));
        }

        [Fact]
        public void ResultTranslationShouldSubstitute()
        {
            Expression<Func<ParentDummy, IEnumerable<ChildDummyView>>> s = d => d.Childs.Select(e => new ChildDummyView { Id = e.Id, Name = e.Name });

            var select = s.Translate().Result((d, v) => new ParentDummyView { Childs = v(d).Take(1) });
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.True(result.Any(v => v.Childs.Any(w => w.Id == 10 && w.Name == "Asdf")));
            Assert.True(result.Any(v => v.Childs.Any(w => w.Id == 11 && w.Name == "Narf")));
            Assert.True(result.Any(v => v.Childs.Any(w => w.Id == 12 && w.Name == "Qwer")));
        }

        [Fact]
        public void CrossTranslationApplyShouldSubstitute()
        {
            Expression<Func<ChildDummy, ChildDummyView>> s = d => new ChildDummyView { Id = d.Id, Name = d.Name };
            Expression<Func<ParentDummy, ParentDummyView>> t = d => new ParentDummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<ParentDummy>((d, v) => d.Childs.Select(e => v(e))).Apply((d, v) => new ParentDummyView { Childs = v(d).Take(1) }, t);
            var result = data.OfType<ParentDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 7 && v.Name == "Asdf" && v.Childs.Any(w => w.Id == 10 && w.Name == "Asdf")));
            Assert.True(result.Any(v => v.Id == 8 && v.Name == "Narf" && v.Childs.Any(w => w.Id == 11 && w.Name == "Narf")));
            Assert.True(result.Any(v => v.Id == 9 && v.Name == "Qwer" && v.Childs.Any(w => w.Id == 12 && w.Name == "Qwer")));
        }
    }
}
