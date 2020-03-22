using System;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => default(IQueryable)!.Rewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).Rewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>)!.Rewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).Rewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable)!.Rewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).Rewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>)!.Rewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).Rewrite(null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).Rewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).Rewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).Rewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).Rewrite(new Rewriter());

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteQueryProvider>(actual.Provider);

            _ = Assert.IsType<Rewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
