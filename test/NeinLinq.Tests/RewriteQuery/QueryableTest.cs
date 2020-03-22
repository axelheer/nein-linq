using System;
using System.Collections;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryableTest
    {
        private readonly Rewriter rewriter;
        private readonly IQueryable<Dummy> query;
        private readonly RewriteQueryProvider provider;

        public QueryableTest()
        {
            rewriter = new Rewriter();

            query = Enumerable.Empty<Dummy>()
                              .AsQueryable()
                              .OrderBy(d => d.Id);

            provider = new RewriteQueryProvider(query.Provider, rewriter);
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteQueryable<Dummy>(null!, provider));
            _ = Assert.Throws<ArgumentNullException>(() => new RewriteQueryable<Dummy>(query, null!));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var actual = ((IEnumerable)new RewriteQueryable<Dummy>(query, provider)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var actual = new RewriteQueryable<Dummy>(query, provider).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteQueryable<Dummy>(query, provider).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteQueryable<Dummy>(query, provider).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteQueryable<Dummy>(query, provider).Provider;

            Assert.Equal(provider, actual);
        }

        [Fact]
        public void QueryShouldReturnQuery()
        {
            var actual = new RewriteQueryable<Dummy>(query, provider).Query;

            Assert.Equal(query, actual);
        }
    }
}
