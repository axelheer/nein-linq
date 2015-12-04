using NeinLinq.Tests.InjectableQueryData;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableQueryBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ToInjectableShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToInjectable();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToInjectableShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToInjectable();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToInjectableShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToInjectable();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ToInjectableShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToInjectable();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
