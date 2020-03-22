using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Fakes.NullsafeQuery;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToEntityNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToEntityNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToEntityNullsafe();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToEntityNullsafe();

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            _ = Assert.IsAssignableFrom<EntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)((RewriteEntityQueryable<Dummy>)actual).Provider;

            _ = Assert.IsType<NullsafeQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
