using System.Linq;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryCleanerTest
    {
        private readonly IAsyncQueryable<Dummy> query;

        public AsyncQueryCleanerTest()
        {
            query = Enumerable.Empty<Dummy>()
                              .ToAsyncEnumerable()
                              .AsAsyncQueryable()
                              .OrderBy(d => d.Id);
        }

        [Fact]
        public async Task ShouldResolveSubQuery()
        {
            var innerRewriter = new Rewriter();
            var outerRewriter = new Rewriter();

            var inner = query.AsyncRewrite(innerRewriter);

            var outer = from dummy in query.AsyncRewrite(outerRewriter)
                        from other in inner
                        select new
                        {
                            dummy,
                            other
                        };

            var result = await outer.ToList();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
        }
    }
}
