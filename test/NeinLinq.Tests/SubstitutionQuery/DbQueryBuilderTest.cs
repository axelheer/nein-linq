using System;
using System.Linq;
using NeinLinq.Fakes.SubstitutionQuery;
using Xunit;

namespace NeinLinq.Tests.SubstitutionQuery
{
    public class DbQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToDbSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToDbSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToDbSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToDbSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToDbSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToDbSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToDbSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToDbSubstitution(typeof(Functions), null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToDbSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToDbSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToDbSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToDbSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteDbQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            _ = Assert.IsType<SubstitutionQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
