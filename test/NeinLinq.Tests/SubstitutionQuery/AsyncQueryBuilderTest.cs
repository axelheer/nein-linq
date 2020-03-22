using System;
using System.Linq;
using NeinLinq.Fakes.SubstitutionQuery;
using Xunit;

namespace NeinLinq.Tests.SubstitutionQuery
{
    public class AsyncQueryBuilderTest
    {
        private readonly object query
            = Enumerable.Empty<Dummy>().ToAsyncEnumerable().AsAsyncQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable)query).ToAsyncSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable)query).ToAsyncSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable<Dummy>)query).ToAsyncSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IAsyncQueryable<Dummy>)query).ToAsyncSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable)query).ToAsyncSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable)query).ToAsyncSubstitution(typeof(Functions), null!));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncSubstitution(null!, typeof(OtherFunctions)));
            _ = Assert.Throws<ArgumentNullException>(() => ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncSubstitution(typeof(Functions), null!));
        }

        [Fact]
        public void ShouldRewriteUntypedQueryable()
        {
            var actual = ((IAsyncQueryable)query).ToAsyncSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedQueryable()
        {
            var actual = ((IAsyncQueryable<Dummy>)query).ToAsyncSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable)query).ToAsyncSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        [Fact]
        public void ShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedAsyncQueryable<Dummy>)query).ToAsyncSubstitution(typeof(Functions), typeof(OtherFunctions));

            AssertQuery(actual);
        }

        private static void AssertQuery(IAsyncQueryable actual)
        {
            _ = Assert.IsType<RewriteAsyncQueryable<Dummy>>(actual);

            var actualProvider = Assert.IsType<RewriteAsyncQueryProvider>(actual.Provider);

            _ = Assert.IsType<SubstitutionQueryRewriter>(actualProvider.Rewriter);
            _ = Assert.IsAssignableFrom<IAsyncQueryProvider>(actualProvider.Provider);
        }
    }
}
