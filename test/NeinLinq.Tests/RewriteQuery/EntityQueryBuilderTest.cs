using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => default(IQueryable)!.EntityRewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).EntityRewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>)!.EntityRewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).EntityRewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable)!.EntityRewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).EntityRewrite(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>)!.EntityRewrite(new Rewriter()));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).EntityRewrite(null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).EntityRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).EntityRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).EntityRewrite(new Rewriter());

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).EntityRewrite(new Rewriter());

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            _ = Assert.IsAssignableFrom<EntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)((RewriteEntityQueryable<Dummy>)actual).Provider;

            _ = Assert.IsType<Rewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
