using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class PredicateTest
    {
        private readonly IQueryable<PredicateDummyA> data;

        public PredicateTest()
        {
            data = new[]
            {
                new PredicateDummyA { Id = 1, Name = "Asdf" },
                new PredicateDummyA { Id = 2, Name = "Narf" },
                new PredicateDummyA { Id = 3, Name = "Qwer" },
                new PredicateDummyB { Id = 4, Name = "Asdf" },
                new PredicateDummyB { Id = 5, Name = "Narf" },
                new PredicateDummyB { Id = 6, Name = "Qwer" },
                new PredicateDummyC { Id = 7, Name = "Asdf" },
                new PredicateDummyC { Id = 8, Name = "Narf" },
                new PredicateDummyC { Id = 9, Name = "Qwer" },
            }
            .AsQueryable();

            data.OfType<PredicateDummyB>().Single(d => d.Id == 4).C = data.OfType<PredicateDummyC>().Where(d => d.Id == 7 || d.Id == 8);
            data.OfType<PredicateDummyB>().Single(d => d.Id == 5).C = data.OfType<PredicateDummyC>().Where(d => d.Id == 8 || d.Id == 9);
            data.OfType<PredicateDummyB>().Single(d => d.Id == 6).C = data.OfType<PredicateDummyC>().Where(d => d.Id == 7 || d.Id == 9);

            data.OfType<PredicateDummyC>().Single(d => d.Id == 7).B = data.OfType<PredicateDummyB>().Single(d => d.Id == 5);
            data.OfType<PredicateDummyC>().Single(d => d.Id == 8).B = data.OfType<PredicateDummyB>().Single(d => d.Id == 6);
            data.OfType<PredicateDummyC>().Single(d => d.Id == 9).B = data.OfType<PredicateDummyB>().Single(d => d.Id == 4);
        }

        [Fact]
        public void AndShouldCombinePredicates()
        {
            Expression<Func<PredicateDummyA, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<PredicateDummyA, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.And(q)).Count();

            Assert.Equal(5, r);
            Assert.Equal(3, s);
            Assert.Equal(1, t);
        }

        [Fact]
        public void OrShouldCombinePredicates()
        {
            Expression<Func<PredicateDummyA, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<PredicateDummyA, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.Or(q)).Count();

            Assert.Equal(5, r);
            Assert.Equal(3, s);
            Assert.Equal(7, t);
        }

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<PredicateDummyA, bool>> p = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.OfType<PredicateDummyB>().Where(p.Translate().To<PredicateDummyB>()).Count();

            Assert.Equal(3, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToPathShouldSubstitute()
        {
            Expression<Func<PredicateDummyB, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<PredicateDummyB>().Where(p).Count();
            var s = data.OfType<PredicateDummyC>().Where(p.Translate().To<PredicateDummyC>(c => c.B)).Count();

            Assert.Equal(1, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToTranslationShouldSubstitute()
        {
            Expression<Func<PredicateDummyC, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<PredicateDummyC>().Where(p).Count();
            var s = data.OfType<PredicateDummyB>().Where(p.Translate().To<PredicateDummyB>((b, q) => b.C.Any(c => q(c)))).Count();

            Assert.Equal(1, r);
            Assert.Equal(2, s);
        }
    }
}
