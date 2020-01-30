using System;
using System.Collections;
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
        public void GetLegacyEnumeratorShouldReturnEnumerator()
        {
            var actual = ((IEnumerable)new RewriteQueryEnumerable<Dummy>(enumerable)).GetEnumerator();

            Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }

        [Fact]
        public void GetEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }

        [Fact]
        public void GetAsyncEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetAsyncEnumerator();

            Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }
    }
}
