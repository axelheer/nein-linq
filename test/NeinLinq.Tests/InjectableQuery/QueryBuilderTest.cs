using System;
using System.Linq;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class QueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable)query).ToInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable<Dummy>)query).ToInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable)query).ToInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable<Dummy>)query).ToInjectable(null!, null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteQueryProvider>(actual.Provider);

            _ = Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
