using System.Linq;
using NeinLinq.Fakes.NullsafeQuery;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class AsyncQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IAsyncQueryable)query).ToAsyncNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IAsyncQueryable<Dummy>)query).ToAsyncNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable)query).ToAsyncNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncNullsafe();

            AssertQuery(actual);
        }

        private static void AssertQuery(IAsyncQueryable actual)
        {
            _ = Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            _ = Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }
    }
}
