using System;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryableTest
    {
        private readonly Rewriter rewriter;
        private readonly IAsyncQueryable<Dummy> query;
        private readonly RewriteAsyncQueryProvider provider;

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
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryable<Dummy>(null, provider));
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryable<Dummy>(query, null));
        }

        [Fact]
        public void GetEnumeratorShouldRewrite()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(query, provider).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(query, provider).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(query, provider).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(query, provider).Provider;

            Assert.Equal(provider, actual);
        }

        [Fact]
        public void QueryShouldReturnQuery()
        {
            var actual = new RewriteAsyncQueryable<Dummy>(query, provider).Query;

            Assert.Equal(query, actual);
        }
    }
}
