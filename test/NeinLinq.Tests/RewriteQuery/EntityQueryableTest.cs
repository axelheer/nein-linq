using System;
using System.Collections;
using System.Linq;
using NeinLinq.EntityFrameworkCore;
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
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryable<Dummy>(provider, null));
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryable<Dummy>(null, query));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldRewrite()
        {
            var actual = ((IEnumerable)new RewriteEntityQueryable<Dummy>(provider, query)).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void GetEnumeratorTypedShouldRewrite()
        {
            var actual = new RewriteEntityQueryable<Dummy>(provider, query).GetEnumerator();

            Assert.NotNull(actual);
            Assert.True(rewriter.VisitCalled);
        }

        [Fact]
        public void ElementTypeShouldReturnElementType()
        {
            var actual = new RewriteEntityQueryable<Dummy>(provider, query).ElementType;

            Assert.Equal(typeof(Dummy), actual);
        }

        [Fact]
        public void ExpressionShouldReturnExpression()
        {
            var actual = new RewriteEntityQueryable<Dummy>(provider, query).Expression;

            Assert.Equal(query.Expression, actual);
        }

        [Fact]
        public void ProviderShouldReturnProvider()
        {
            var actual = new RewriteEntityQueryable<Dummy>(provider, query).Provider;

            Assert.IsType<RewriteEntityQueryProvider>(actual);
            Assert.IsType<Rewriter>(((RewriteEntityQueryProvider)actual).Rewriter);
        }
    }
}
