using System;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryEnumerableTest
    {
        private readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetAsyncEnumerator();

            Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }
    }
}
