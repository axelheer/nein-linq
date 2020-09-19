using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Fakes.InjectableQuery;
using Xunit;

#pragma warning disable EF1001

namespace NeinLinq.Tests.InjectableQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToEntityInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable)query).ToEntityInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToEntityInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable<Dummy>)query).ToEntityInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToEntityInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable)query).ToEntityInjectable(null!, null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToEntityInjectable(null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable<Dummy>)query).ToEntityInjectable(null!, null!));
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
            _ = Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            _ = Assert.IsAssignableFrom<EntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)((RewriteEntityQueryable<Dummy>)actual).Provider;

            _ = Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
