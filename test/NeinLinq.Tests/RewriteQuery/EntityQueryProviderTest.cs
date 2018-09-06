using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryProviderTest
    {
        private readonly IQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryProvider(query.Provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryProvider(null, new Rewriter()));
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
            Assert.IsType<RewriteEntityQueryable<Dummy>>(actual);
            Assert.IsType<RewriteEntityQueryProvider>(actual.Provider);

            var actualProvider = (RewriteEntityQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
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
