using System;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IAsyncQueryable).AsyncRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable)query).AsyncRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IAsyncQueryable<Dummy>).AsyncRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable<Dummy>)query).AsyncRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedAsyncQueryable).AsyncRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable)query).AsyncRewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedAsyncQueryable<Dummy>).AsyncRewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable<Dummy>)query).AsyncRewrite(null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IAsyncQueryable)query).AsyncRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IAsyncQueryable<Dummy>)query).AsyncRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable)query).AsyncRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable<Dummy>)query).AsyncRewrite(new Rewriter());

            AssertQuery(actual);
        }

        private static void AssertQuery(IAsyncQueryable actual)
        {
            Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);
            Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            var actualProvider = (RewriteAsyncQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }
    }
}
