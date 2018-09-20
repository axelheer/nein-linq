using System;
using System.Collections;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryableTest
    {
        private readonly Rewriter rewriter;
        private readonly IQueryable<Dummy> query;
        private readonly RewriteDbQueryProvider provider;

        public DbQueryableTest()
        {
            rewriter = new Rewriter();

            query = Enumerable.Empty<Dummy>()
                              .AsQueryable()
                              .OrderBy(d => d.Id);

            provider = new RewriteDbQueryProvider(query.Provider, rewriter);
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryable<Dummy>(null, provider));
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryable<Dummy>(query, null));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var actual = ((IEnumerable)new RewriteDbQueryable<Dummy>(query, provider)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var actual = new RewriteDbQueryable<Dummy>(query, provider).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteDbQueryable<Dummy>(query, provider).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteDbQueryable<Dummy>(query, provider).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteDbQueryable<Dummy>(query, provider).Provider;

            Assert.IsType<RewriteDbQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteDbQueryProvider)actual).Rewriter);
        }
    }
}
