using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryProviderTest
    {
        private readonly IQueryable<Dummy> query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryProvider(query.Provider, null!));
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryProvider(null!, new Rewriter()));
        }

        [Fact]
        public void CreateQueryUntypedShouldRewrite()
        {
            var actual = new RewriteEntityQueryProvider(query.Provider, new Rewriter()).CreateQuery(query.Expression);

            AssertQuery(actual);
        }

        [Fact]
        public void CreateQueryTypedShouldRewrite()
        {
            var actual = new RewriteEntityQueryProvider(query.Provider, new Rewriter()).CreateQuery<Dummy>(query.Expression);

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

        [Fact]
        public void ExecuteUntypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteEntityQueryProvider(query.Provider, rewriter).Execute(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ExecuteTypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteEntityQueryProvider(query.Provider, rewriter).Execute<int>(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }
    }
}
