using NeinLinq.Tests.NullsafeQueryData;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class NullsafeQueryBuilderTest
    {
        readonly IOrderedQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.SomeNumeric);

        [Fact]
        public void ToNullsafeShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToNullsafeShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToNullsafeShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToNullsafeShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToNullsafe();

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }
    }
}
