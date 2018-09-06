using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class AsyncQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable)query).ToAsyncInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IAsyncQueryable)query).ToAsyncInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable<Dummy>)query).ToAsyncInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IAsyncQueryable<Dummy>)query).ToAsyncInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable)query).ToAsyncInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedAsyncQueryable)query).ToAsyncInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncInjectable(null, null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IAsyncQueryable)query).ToAsyncInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IAsyncQueryable<Dummy>)query).ToAsyncInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable)query).ToAsyncInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncInjectable();

            AssertQuery(actual);
        }

        private static void AssertQuery(IAsyncQueryable actual)
        {
            Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);
            Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            var actualProvider = (RewriteAsyncQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }
    }
}
