using NeinLinq.Tests.RewriteQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class RewriteQueryBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IQueryable).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).Rewrite(null));
        }

        [Fact]
        public void RewriteShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void RewriteShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void RewriteShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void RewriteShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
