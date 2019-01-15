using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryCleanerTest
    {
        private readonly IQueryable<Dummy> query;

        public QueryCleanerTest()
        {
            query = Enumerable.Empty<Dummy>()
                              .AsQueryable()
                              .OrderBy(d => d.Id);
        }

        [Fact]
        public void ShouldResolveSubQuery()
        {
            var innerRewriter = new Rewriter();
            var outerRewriter = new Rewriter();

            var inner = query.Rewrite(innerRewriter);

            var outer = from dummy in query.Rewrite(outerRewriter)
                        from other in inner
                        select new
                        {
                            dummy,
                            other
                        };

            var result = outer.ToList();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
        }
    }
}
