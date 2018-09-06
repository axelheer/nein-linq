using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class DbQueryBuilderTest
    {
        private readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToDbInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable)query).ToDbInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToDbInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable<Dummy>)query).ToDbInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToDbInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable)query).ToDbInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToDbInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable<Dummy>)query).ToDbInjectable(null, null));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToDbInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToDbInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToDbInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToDbInjectable();

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteDbQueryable<Dummy>>(actual);
            Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            var actualProvider = (RewriteDbQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
