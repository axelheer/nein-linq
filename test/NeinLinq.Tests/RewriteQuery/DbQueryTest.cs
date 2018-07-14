using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.EntityFramework;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryTest
    {
        readonly IQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQuery<Dummy>(query, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQuery<Dummy>(null, new Rewriter()));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var rewriter = new Rewriter();

            var actual = ((RewriteDbQuery)new RewriteDbQuery<Dummy>(query, rewriter)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var rewriter = new Rewriter();

            var actual = new RewriteDbQuery<Dummy>(query, rewriter).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteDbQuery<Dummy>(query, new Rewriter()).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteDbQuery<Dummy>(query, new Rewriter()).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteDbQuery<Dummy>(query, new Rewriter()).Provider;

            Assert.IsType<RewriteDbQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteDbQueryProvider)actual).Rewriter);
        }
    }
}
