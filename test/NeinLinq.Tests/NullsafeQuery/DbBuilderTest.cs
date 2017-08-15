#if NETFRAMEWORK

using NeinLinq.EntityFramework;
using NeinLinq.Fakes.NullsafeQuery;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class DbBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToNullsafe();

            AssertQuery(actual);
        }

        static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteDbQuery<Dummy>>(actual);
            Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            var actualProvider = (RewriteDbQueryProvider)actual.Provider;

            Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}

#endif
