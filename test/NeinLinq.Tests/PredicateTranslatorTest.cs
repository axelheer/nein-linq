using NeinLinq.Tests.PredicateTranslatorData;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class PredicateTranslatorTest
    {
        readonly IQueryable<IDummy> data;

        public PredicateTranslatorTest()
        {
            var d = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" }
            };
            var s = new[]
            {
                new SuperDummy { Id = 4, Name = "Asdf" },
                new SuperDummy { Id = 5, Name = "Narf" },
                new SuperDummy { Id = 6, Name = "Qwer" }
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
            p[2].Children = new[] { c[0], c[2] };

            data = d.Concat<IDummy>(s).Concat(p).Concat(c).AsQueryable();
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new PredicateTranslation<Dummy>(null));
        }

        [Fact]
        public void AndShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>> p = _ => false;
            Expression<Func<IDummy, bool>> q = null;

            Assert.Throws<ArgumentNullException>(() => p.And(q));
            Assert.Throws<ArgumentNullException>(() => q.And(p));
        }

        [Fact]
        public void AndShouldCombinePredicates()
        {
            Expression<Func<IDummy, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<IDummy, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.And(q)).Count();

            Assert.Equal(6, r);
            Assert.Equal(4, s);
            Assert.Equal(2, t);
        }

        [Fact]
        public void OrShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>> p = _ => false;
            Expression<Func<IDummy, bool>> q = null;

            Assert.Throws<ArgumentNullException>(() => p.Or(q));
            Assert.Throws<ArgumentNullException>(() => q.Or(p));
        }

        [Fact]
        public void OrShouldCombinePredicates()
        {
            Expression<Func<IDummy, bool>> p = d => d.Id % 2 == 1;
            Expression<Func<IDummy, bool>> q = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(q).Count();
            var t = data.Where(p.Or(q)).Count();

            Assert.Equal(6, r);
            Assert.Equal(4, s);
            Assert.Equal(8, t);
        }

        [Fact]
        public void NotShouldHandleInvalidArguments()
        {
            Expression<Func<IDummy, bool>> p = null;

            Assert.Throws<ArgumentNullException>(() => p.Not());
        }

        [Fact]
        public void NotShouldNegatePredicate()
        {
            Expression<Func<IDummy, bool>> p = d => d.Name == "Narf";

            var r = data.Where(p).Count();
            var s = data.Where(p.Not()).Count();

            Assert.Equal(4, r);
            Assert.Equal(8, s);
        }

        [Fact]
        public void TranslateShouldHandleInvalidArguments()
        {
            Expression<Func<Dummy, bool>> p = null;

            Assert.Throws<ArgumentNullException>(() => p.Translate());
        }

        [Fact]
        public void ToSubtypeShouldSubstitute()
        {
            Expression<Func<Dummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<Dummy>().Where(p).Count();
            var s = data.OfType<SuperDummy>().Where(p.Translate().To<SuperDummy>()).Count();

            Assert.Equal(2, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToPathShouldHandleInvalidArguments()
        {
            Expression<Func<ParentDummy, bool>> p = _ => false;

            Assert.Throws<ArgumentNullException>(() => p.Translate().To(default(Expression<Func<ChildDummy, ParentDummy>>)));
        }

        [Fact]
        public void ToPathShouldSubstitute()
        {
            Expression<Func<ParentDummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<ParentDummy>().Where(p).Count();
            var s = data.OfType<ChildDummy>().Where(p.Translate().To<ChildDummy>(c => c.Parent)).Count();

            Assert.Equal(1, r);
            Assert.Equal(1, s);
        }

        [Fact]
        public void ToTranslationShouldHandleInvalidArguments()
        {
            Expression<Func<ChildDummy, bool>> p = _ => false;

            Assert.Throws<ArgumentNullException>(() => p.Translate().To(default(Expression<Func<ParentDummy, Func<ChildDummy, bool>, bool>>)));
        }

        [Fact]
        public void ToTranslationShouldSubstitute()
        {
            Expression<Func<ChildDummy, bool>> p = d => d.Name == "Narf";

            var r = data.OfType<ChildDummy>().Where(p).Count();
            var s = data.OfType<ParentDummy>().Where(p.Translate().To<ParentDummy>((b, q) => b.Children.Any(q))).Count();

            Assert.Equal(1, r);
            Assert.Equal(2, s);
        }
    }
}
