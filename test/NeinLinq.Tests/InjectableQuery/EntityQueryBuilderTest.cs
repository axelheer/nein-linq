using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToEntityInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable)query).ToEntityInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToEntityInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable<Dummy>)query).ToEntityInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToEntityInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable)query).ToEntityInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToEntityInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable<Dummy>)query).ToEntityInjectable(null, null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToEntityInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToEntityInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToEntityInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToEntityInjectable();

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            Assert.IsType<RewriteEntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
