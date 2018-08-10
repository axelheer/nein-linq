using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Queryable;
using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryTest
    {
        readonly IQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQuery<Dummy>(query, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteQuery<Dummy>(null, new Rewriter()));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var rewriter = new Rewriter();

            var actual = ((IEnumerable)new RewriteQuery<Dummy>(query, rewriter)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var rewriter = new Rewriter();

            var actual = new RewriteQuery<Dummy>(query, rewriter).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteQuery<Dummy>(query, new Rewriter()).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteQuery<Dummy>(query, new Rewriter()).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteQuery<Dummy>(query, new Rewriter()).Provider;

            Assert.IsType<RewriteQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteQueryProvider)actual).Rewriter);
        }
    }
}
