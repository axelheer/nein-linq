using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryCleanerTest
    {
        private readonly IQueryable<Dummy> query;

        public EntityQueryCleanerTest()
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

            var inner = query.EntityRewrite(innerRewriter);

            var outer = from dummy in query.EntityRewrite(outerRewriter)
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
