using System;
using System.Linq;
using NeinLinq.EntityFrameworkCore;
using NeinLinq.Fakes.SubstitutionQuery;
using Xunit;

namespace NeinLinq.Tests.SubstitutionQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToSubstitution(typeof(Functions), null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            Assert.IsType<RewriteEntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)actual.Provider;

            Assert.IsType<SubstitutionQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
