using System;
using System.Collections;
using System.Linq;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryableTest
    {
        private readonly Rewriter rewriter;
        private readonly IQueryable<Dummy> query;
        private readonly RewriteEntityQueryProvider provider;

        public EntityQueryableTest()
        {
            rewriter = new Rewriter();

            query = Enumerable.Empty<Dummy>()
                              .AsQueryable()
                              .OrderBy(d => d.Id);

            provider = new RewriteEntityQueryProvider(query.Provider, rewriter);
        }

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryable<Dummy>(null, provider));
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryable<Dummy>(query, null));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var actual = ((IEnumerable)new RewriteEntityQueryable<Dummy>(query, provider)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var actual = new RewriteEntityQueryable<Dummy>(query, provider).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteEntityQueryable<Dummy>(query, provider).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteEntityQueryable<Dummy>(query, provider).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteEntityQueryable<Dummy>(query, provider).Provider;

            Assert.Equal(provider, actual);
        }

        [Fact]
        public void QueryableShouldReturnQueryable()
        {
            var actual = new RewriteEntityQueryable<Dummy>(query, provider).Queryable;

            Assert.Equal(query, actual);
        }
    }
}
