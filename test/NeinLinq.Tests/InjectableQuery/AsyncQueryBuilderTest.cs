using NeinLinq.Fakes.InjectableQuery;
using NeinLinq.Interactive;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class AsyncQueryBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IAsyncQueryable)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable<Dummy>)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IAsyncQueryable<Dummy>)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedAsyncQueryable)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToInjectable(null, null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IAsyncQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IAsyncQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        static void AssertQuery(IAsyncQueryable actual)
        {
            Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);
            Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            var actualProvider = (RewriteAsyncQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }
    }
}
