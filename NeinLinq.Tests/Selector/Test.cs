using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.Selector
{
    public class Test
    {
        private readonly IQueryable<Dummy> data;

        public Test()
        {
            var d = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" }
            };
            var s = new[]
            {
                new SuperDummy { Id = 3, Name = "Asdf", Description = "Narf" },
                new SuperDummy { Id = 4, Name = "Narf", Description = "Asdf" }
            };
            var o = new[]
            {
                new DummyOne { Id = 5, Name = "Asdf" },
                new DummyOne { Id = 6, Name = "Narf" },
            };
            var t = new[]
            {
                new DummyTwo { Id = 7, Name = "Asdf", One = o[0] },
                new DummyTwo { Id = 8, Name = "Narf", One = o[1] }
            };
            o[0].Twos = new[] { t[0], t[1] };
            o[1].Twos = new[] { t[1], t[0] };

            data = d.Concat(s).Concat(o).Concat(t).AsQueryable();
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
        }

        [Fact]
        public void SourceSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<SuperDummy>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Narf"));
        }

        [Fact]
        public void ResultSubtypeShouldSubstitute()
        {
            Expression<Func<SuperDummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Result<SuperDummyView>();
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Asdf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Narf" && v.Description == null));
        }

        [Fact]
        public void CrossSubtypeApplyShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = d => new SuperDummyView { Description = d.Description };

            var select = s.Translate().Cross<SuperDummy>().Apply(t);
            var result = data.OfType<SuperDummy>().Select(select);

            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Asdf" && v.Description == "Narf"));
            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Narf" && v.Description == "Asdf"));
        }

        [Fact]
        public void SourcePathShouldSubstitute()
        {
            Expression<Func<DummyOne, DummyOneView>> s = d => new DummyOneView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<DummyTwo>(d => d.One);
            var result = data.OfType<DummyTwo>().Select(select);

            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Narf"));
        }

        [Fact]
        public void ResultPathShouldSubstitute()
        {
            Expression<Func<DummyTwo, DummyOneView>> s = d => new DummyOneView { Id = d.One.Id, Name = d.One.Name };

            var select = s.Translate().Result<DummyTwoView>(d => d.One);
            var result = data.OfType<DummyTwo>().Select(select);

            Assert.True(result.Any(v => v.One.Id == 5 && v.One.Name == "Asdf"));
            Assert.True(result.Any(v => v.One.Id == 6 && v.One.Name == "Narf"));
        }

        [Fact]
        public void CrossPathApplyShouldSubstitute()
        {
            Expression<Func<DummyOne, DummyOneView>> s = d => new DummyOneView { Id = d.Id, Name = d.Name };
            Expression<Func<DummyTwo, DummyTwoView>> t = d => new DummyTwoView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<DummyTwo>(d => d.One).Apply(d => d.One, t);
            var result = data.OfType<DummyTwo>().Select(select);

            Assert.True(result.Any(v => v.One.Id == 5 && v.One.Name == "Asdf" && v.Id == 7 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.One.Id == 6 && v.One.Name == "Narf" && v.Id == 8 && v.Name == "Narf"));
        }

        [Fact(Skip = "Not yet implemented.")]
        public void SourceTranslationShouldSubstitute()
        {
            Expression<Func<DummyTwo, DummyTwoView>> s = d => new DummyTwoView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Source<DummyOne>((d, v) => d.Twos.Select(e => v(e)));
            var result = data.OfType<DummyOne>().Select(select);

            Assert.True(result.All(v => v.Any(w => w.Id == 7 && w.Name == "Asdf")));
            Assert.True(result.All(v => v.Any(w => w.Id == 8 && w.Name == "Narf")));
        }

        [Fact(Skip = "Not yet implemented.")]
        public void ResultTranslationShouldSubstitute()
        {
            Expression<Func<DummyOne, IEnumerable<DummyTwoView>>> s = d => d.Twos.Select(e => new DummyTwoView { Id = e.Id, Name = e.Name });

            var select = s.Translate().Result((d, v) => new DummyOneView { Twos = v(d) });
            var result = data.OfType<DummyOne>().Select(select);

            Assert.True(result.All(v => v.Twos.Any(w => w.Id == 7 && w.Name == "Asdf")));
            Assert.True(result.All(v => v.Twos.Any(w => w.Id == 8 && w.Name == "Narf")));
        }

        [Fact(Skip = "Not yet implemented.")]
        public void CrossTranslationApplyShouldSubstitute()
        {
            Expression<Func<DummyTwo, DummyTwoView>> s = d => new DummyTwoView { Id = d.Id, Name = d.Name };
            Expression<Func<DummyOne, DummyOneView>> t = d => new DummyOneView { Id = d.Id, Name = d.Name };

            var select = s.Translate().Cross<DummyOne>((d, v) => d.Twos.Select(e => v(e))).Apply((d, v) => new DummyOneView { Twos = v(d) }, t);
            var result = data.OfType<DummyOne>().Select(select);

            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Asdf" && v.Twos.Any(w => w.Id == 7 && w.Name == "Asdf")));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Asdf" && v.Twos.Any(w => w.Id == 8 && w.Name == "Narf")));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Narf" && v.Twos.Any(w => w.Id == 7 && w.Name == "Asdf")));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Narf" && v.Twos.Any(w => w.Id == 8 && w.Name == "Narf")));
        }
    }
}
