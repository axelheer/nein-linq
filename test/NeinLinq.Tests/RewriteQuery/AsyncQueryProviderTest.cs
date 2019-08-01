using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryProviderTest
    {
        private readonly IAsyncQueryable<Dummy> query = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryProvider(query.Provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryProvider(null, new Rewriter()));
        }

        [Fact]
        public void CreateQueryShouldRewrite()
        {
            var actual = new RewriteAsyncQueryProvider(query.Provider, new Rewriter()).CreateQuery<Dummy>(query.Expression);

            AssertQuery(actual);
        }

        private static void AssertQuery(IAsyncQueryable actual)
        {
            Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);
            Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            var actualProvider = (RewriteAsyncQueryProvider)actual.Provider;

            Assert.IsType<Rewriter>(actualProvider.Rewriter);
            Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }

        [Fact]
        public async Task ExecutedShouldRewrite()
        {
            var rewriter = new Rewriter();
            var expression = Expression.Call(typeof(AsyncQueryable), nameof(AsyncQueryable.CountAsync), new[] { typeof(Dummy) }, query.Expression, Expression.Default(typeof(CancellationToken)));

            var actual = await new RewriteAsyncQueryProvider(query.Provider, rewriter).ExecuteAsync<int>(expression, CancellationToken.None);

            Assert.Equal(0, actual);
            Assert.True(rewriter.VisitCalled);
        }
    }
}
