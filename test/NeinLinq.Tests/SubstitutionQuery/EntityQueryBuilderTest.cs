using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Fakes.SubstitutionQuery;
using Xunit;

#pragma warning disable EF1001

namespace NeinLinq.Tests.SubstitutionQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToEntitySubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToEntitySubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToEntitySubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToEntitySubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToEntitySubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToEntitySubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToEntitySubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToEntitySubstitution(typeof(Functions), null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToEntitySubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToEntitySubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToEntitySubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToEntitySubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            _ = Assert.IsAssignableFrom<EntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)((RewriteEntityQueryable<Dummy>)actual).Provider;

            _ = Assert.IsType<SubstitutionQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
