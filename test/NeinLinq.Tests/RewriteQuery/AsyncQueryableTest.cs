using System;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Interactive;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryableTest
    {
        readonly Rewriter rewriter;
        readonly IAsyncQueryable<Dummy> query;
        readonly RewriteAsyncQueryProvider provider;

        public AsyncQueryableTest()
        {
            rewriter = new Rewriter();

            query = Enumerable.Empty<Dummy>()
                              .ToAsyncEnumerable()
                              .AsAsyncQueryable()
                              .OrderBy(d => d.Id);

            provider = new RewriteAsyncQueryProvider(query.Provider, rewriter);
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryable<Dummy>(provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryable<Dummy>(null, query));
        }

        [Fact]
        public void GetEnumeratorShouldRewrite()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(provider, query).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(provider, query).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(provider, query).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(provider, query).Provider;

            Assert.IsType<RewriteAsyncQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteAsyncQueryProvider)actual).Rewriter);
        }
    }
}
