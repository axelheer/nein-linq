using System.Linq;
using NeinLinq.Fakes.NullsafeQuery;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class DbQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToDbNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToDbNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToDbNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToDbNullsafe();

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteDbQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            _ = Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
