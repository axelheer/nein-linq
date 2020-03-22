using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryProviderTest
    {
        private readonly IQueryable<Dummy> query
            = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryProvider(query.Provider, null!));
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryProvider(null!, new Rewriter()));
        }

        [Fact]
        public void CreateQueryUntypedShouldRewrite()
        {
            var actual = new RewriteDbQueryProvider(query.Provider, new Rewriter()).CreateQuery(query.Expression);

            AssertQuery(actual);
        }

        [Fact]
        public void CreateQueryTypedShouldRewrite()
        {
            var actual = new RewriteDbQueryProvider(query.Provider, new Rewriter()).CreateQuery<Dummy>(query.Expression);

            AssertQuery(actual);
        }

        private static void AssertQuery(IQueryable actual)
        {
            _ = Assert.IsType<RewriteDbQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteDbQueryProvider>(actual.Provider);

            _ = Assert.IsType<Rewriter>(actualProvider.Rewriter);
            _ = Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ExecuteUntypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteDbQueryProvider(query.Provider, rewriter).Execute(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ExecuteTypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteDbQueryProvider(query.Provider, rewriter).Execute<int>(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }
    }
}
