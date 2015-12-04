using NeinLinq.Tests.NullsafeQueryData;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class NullsafeQueryBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ToNullsafeShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToNullsafeShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToNullsafeShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToNullsafeShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
