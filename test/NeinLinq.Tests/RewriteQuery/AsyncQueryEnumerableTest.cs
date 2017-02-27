using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Interactive;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class AsyncQueryEnumerableTest
    {
        readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorShouldReturnEnumerator()
        {
            var actual = new RewriteAsyncQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            Assert.IsType<RewriteAsyncQueryEnumerator<Dummy>>(actual);
        }

        [Fact]
        public void DisposeShouldCallDispose()
        {
            new RewriteAsyncQueryEnumerable<Dummy>(enumerable).Dispose();

            Assert.True(enumerable.DisposeCalled);
        }
    }
}
