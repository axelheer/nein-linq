using System;
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
            Expression<Func<Dummy, DummyView>> t = e => new DummyView { Name = e.Name };

            var result = data.Select(s.Apply(t));

            Assert.True(result.Any(v => v.Id == 1 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 2 && v.Name == "Narf"));
        }

        [Fact]
        public void SourceSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var result = data.OfType<SuperDummy>().Select(s.Translate().Source<SuperDummy>());

            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Narf"));
        }

        [Fact]
        public void SourcePathShouldSubstitute()
        {
            Expression<Func<DummyOne, DummyOneView>> s = d => new DummyOneView { Id = d.Id, Name = d.Name };
            Expression<Func<DummyTwo, DummyOne>> u = e => e.One;

            var result = data.OfType<DummyTwo>().Select(s.Translate().Source<DummyTwo>(u));

            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Narf"));
        }

        [Fact]
        public void ResultSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var result = data.Select(s.Translate().Result<SuperDummyView>());

            Assert.True(result.Any(v => v.Id == 1 && v.Name == "Asdf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 2 && v.Name == "Narf" && v.Description == null));
        }

        [Fact]
        public void ResultPathShouldSubstitute()
        {
            Expression<Func<DummyOne, DummyOneView>> s = d => new DummyOneView { Id = d.Id, Name = d.Name };
            Expression<Func<DummyTwoView, DummyOneView>> u = e => e.One;

            var result = data.OfType<DummyOne>().Select(s.Translate().Result<DummyTwoView>(u));

            Assert.True(result.Any(v => v.One.Id == 5 && v.One.Name == "Asdf"));
            Assert.True(result.Any(v => v.One.Id == 6 && v.One.Name == "Narf"));
        }

        [Fact]
        public void CrossSubtypeApplyShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = e => new SuperDummyView { Description = e.Description };

            var result = data.OfType<SuperDummy>().Select(s.Translate().Cross<SuperDummy>().Apply(t));

            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Asdf" && v.Description == "Narf"));
            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Narf" && v.Description == "Asdf"));
        }

        [Fact]
        public void CrossPathApplyShouldSubstitute()
        {
            Expression<Func<DummyTwoView, DummyOneView>> r = e => e.One;
            Expression<Func<DummyOne, DummyOneView>> s = d => new DummyOneView { Id = d.Id, Name = d.Name };
            Expression<Func<DummyTwo, DummyTwoView>> t = e => new DummyTwoView { Id = e.Id, Name = e.Name };
            Expression<Func<DummyTwo, DummyOne>> u = e => e.One;

            var result = data.OfType<DummyTwo>().Select(s.Translate().Cross<DummyTwo>(u).Apply(r, t));

            Assert.True(result.Any(v => v.Id == 7 && v.Name == "Asdf" && v.One.Id == 5 && v.One.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 8 && v.Name == "Narf" && v.One.Id == 6 && v.One.Name == "Narf"));
        }
    }
}
