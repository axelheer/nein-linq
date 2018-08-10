using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Queryable;
using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryableTest
    {
        readonly Rewriter rewriter;
        readonly IQueryable<Dummy> query;
        readonly RewriteQueryProvider provider;

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
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryable<Dummy>(provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryable<Dummy>(null, query));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var actual = ((IEnumerable)new RewriteQueryable<Dummy>(provider, query)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var actual = new RewriteQueryable<Dummy>(provider, query).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteQueryable<Dummy>(provider, query).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteQueryable<Dummy>(provider, query).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteQueryable<Dummy>(provider, query).Provider;

            Assert.IsType<RewriteQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteQueryProvider)actual).Rewriter);
        }
    }
}
