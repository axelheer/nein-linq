using System;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IQueryable).DbRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).DbRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).DbRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).DbRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable).DbRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).DbRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>).DbRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).DbRewrite(null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).DbRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).DbRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).DbRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).DbRewrite(new Rewriter());

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteDbQueryable<Dummy>>(actual);
            Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            var actualProvider = (RewriteDbQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
