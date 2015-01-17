using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.Predicate
{
    public class Test
    {
        private readonly IQueryable<Dummy> data;

        public Test()
        {
            var d = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" }
            };
            var o = new[]
            {
                new DummyOne { Id = 4, Name = "Asdf" },
                new DummyOne { Id = 5, Name = "Narf" },
                new DummyOne { Id = 6, Name = "Qwer" }
            };
            var t = new[]
            {
                new DummyTwo { Id = 7, Name = "Asdf", One = o[1] },
                new DummyTwo { Id = 8, Name = "Narf", One = o[2] },
                new DummyTwo { Id = 9, Name = "Qwer", One = o[0] }
            };
            o[0].Twos = new[] { t[0], t[1] };
            o[1].Twos = new[] { t[1], t[2] };
            o[2].Twos = new[] { t[0], t[2] };

            data = d.Concat(o).Concat(t).AsQueryable();
        }

        [Fact]
        public void AndShouldCombinePredicates()
        {
            Expression<Func<Dummy, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<Dummy, bool>> q = d => d.Name == "Narf";

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
            Expression<Func<Dummy, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<Dummy, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.Or(q)).Count();

            Assert.Equal(5, r);
            Assert.Equal(3, s);
            Assert.Equal(7, t);
        }

        [Fact]
        public void NotShouldNegatePredicate()
        {
            Expression<Func<Dummy, bool>> p = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(p.Not()).Count();

            Assert.Equal(3, r);
            Assert.Equal(6, s);
        }

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, bool>> p = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.OfType<DummyOne>().Where(p.Translate().To<DummyOne>()).Count();

            Assert.Equal(3, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToPathShouldSubstitute()
        {
            Expression<Func<DummyOne, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<DummyOne>().Where(p).Count();
            var s = data.OfType<DummyTwo>().Where(p.Translate().To<DummyTwo>(c => c.One)).Count();

            Assert.Equal(1, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToTranslationShouldSubstitute()
        {
            Expression<Func<DummyTwo, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<DummyTwo>().Where(p).Count();
            var s = data.OfType<DummyOne>().Where(p.Translate().To<DummyOne>((b, q) => b.Twos.Any(c => q(c)))).Count();

            Assert.Equal(1, r);
            Assert.Equal(2, s);
        }
    }
}
