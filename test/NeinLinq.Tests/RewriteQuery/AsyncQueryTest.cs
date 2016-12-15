using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Interactive;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryTest
    {
        readonly IAsyncQueryable<Dummy> query = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQuery<Dummy>(query, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQuery<Dummy>(null, new Rewriter()));
        }

        [Fact]
        public void GetEnumeratorShouldRewrite()
        {
            var rewriter = new Rewriter();

            var actual = new RewriteAsyncQuery<Dummy>(query, rewriter).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteAsyncQuery<Dummy>(query, new Rewriter()).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteAsyncQuery<Dummy>(query, new Rewriter()).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteAsyncQuery<Dummy>(query, new Rewriter()).Provider;

            Assert.IsType<RewriteAsyncQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteAsyncQueryProvider)actual).Rewriter);
        }
    }
}
