using System;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryEnumerableTest
    {
        private readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteEntityQueryEnumerable<Dummy>(enumerable).GetAsyncEnumerator();

            Assert.IsType<RewriteEntityQueryEnumerator<Dummy>>(actual);
        }
    }
}
