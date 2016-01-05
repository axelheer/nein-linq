using NeinLinq.Tests.InjectableQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class InjectableQueryBuilderTest
    {
        readonly object query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ToInjectableShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IQueryable<Dummy>)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable)query).ToInjectable(null, null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToInjectable(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((IOrderedQueryable<Dummy>)query).ToInjectable(null, null));
        }

        [Fact]
        public void ToInjectableShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ToInjectableShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ToInjectableShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToInjectable();

            AssertQuery(actual);
        }

        [Fact]
        public void ToInjectableShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToInjectable();

            AssertQuery(actual);
        }

        static void AssertQuery(IQueryable actual)
        {
            Assert.IsType<RewriteQuery<Dummy>>(actual);
            Assert.IsType<RewriteQueryProvider>(actual.Provider);

            var actualProvider = (RewriteQueryProvider)actual.Provider;

            Assert.IsType<InjectableQueryRewriter>(actualProvider.Rewriter);
            Assert.IsType<EnumerableQuery<Dummy>>(actualProvider.Provider);
        }
    }
}
