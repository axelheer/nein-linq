using System;
using System.Collections;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public sealed class QueryEnumerableTest : IDisposable
    {
        private readonly DummyEnumerable enumerable
            = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(()
                => new RewriteQueryEnumerable<Dummy>(null!));
        }

        [Fact]
        public void GetLegacyEnumeratorShouldReturnEnumerator()
        {
            var actual = ((IEnumerable)new RewriteQueryEnumerable<Dummy>(enumerable)).GetEnumerator();

            _ = Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }

        [Fact]
        public void GetEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            _ = Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }

        [Fact]
        public void GetAsyncEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetAsyncEnumerator();

            _ = Assert.IsType<RewriteQueryEnumerator<Dummy>>(actual);
        }

        public void Dispose()
            => enumerable.Dispose();
    }
}
