using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class ProviderTest
    {
        readonly IQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryProvider(query.Provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryProvider(null, new Rewriter()));
        }

        [Fact]
        public void CreateQueryUntypedShouldRewrite()
        {
            var actual = new RewriteQueryProvider(query.Provider, new Rewriter()).CreateQuery(query.Expression);

            AssertQuery(actual);
        }

        [Fact]
        public void CreateQueryTypedShouldRewrite()
        {
            var actual = new RewriteQueryProvider(query.Provider, new Rewriter()).CreateQuery<Dummy>(query.Expression);

            AssertQuery(actual);
        }

        static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }

        [Fact]
        public void ExecuteUntypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteQueryProvider(query.Provider, rewriter).Execute(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ExecuteTypedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var actual = new RewriteQueryProvider(query.Provider, rewriter).Execute<int>(expression);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }
    }
}
