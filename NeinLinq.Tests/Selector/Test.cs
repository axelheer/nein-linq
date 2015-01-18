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
            data = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" },
                new SuperDummy { Id = 4, Name = "Asdf", Description = "Qwer" },
                new SuperDummy { Id = 5, Name = "Narf", Description = "Asdf" },
                new SuperDummy { Id = 6, Name = "Qwer", Description = "Narf" }
            }
            .AsQueryable();
        }

        [Fact]
        public void ApplyShouldMergeSelectors()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id };
            Expression<Func<Dummy, DummyView>> t = e => new DummyView { Name = e.Name };

            var result = data.Select(s.Apply(t));

            Assert.True(result.Any(v => v.Id == 1 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 2 && v.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Qwer"));
        }

        [Fact]
        public void ResultShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var result = data.Select(s.Translate().Result<SuperDummyView>());

            Assert.True(result.Any(v => v.Id == 1 && v.Name == "Asdf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 2 && v.Name == "Narf" && v.Description == null));
            Assert.True(result.Any(v => v.Id == 3 && v.Name == "Qwer" && v.Description == null));
        }

        [Fact]
        public void SourceShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };

            var result = data.OfType<SuperDummy>().Select(s.Translate().Source<SuperDummy>());

            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Asdf"));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Narf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Qwer"));
        }

        [Fact]
        public void CrossApplyShouldSubstitute()
        {
            Expression<Func<Dummy, DummyView>> s = d => new DummyView { Id = d.Id, Name = d.Name };
            Expression<Func<SuperDummy, SuperDummyView>> t = e => new SuperDummyView { Description = e.Description };

            var result = data.OfType<SuperDummy>().Select(s.Translate().Cross<SuperDummy>().Apply(t));

            Assert.True(result.Any(v => v.Id == 4 && v.Name == "Asdf" && v.Description == "Qwer"));
            Assert.True(result.Any(v => v.Id == 5 && v.Name == "Narf" && v.Description == "Asdf"));
            Assert.True(result.Any(v => v.Id == 6 && v.Name == "Qwer" && v.Description == "Narf"));
        }
    }
}
