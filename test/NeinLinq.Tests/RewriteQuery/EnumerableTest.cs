using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EnumerableTest
    {
        readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldReturnEnumerator()
        {
            var actual = ((RewriteQueryEnumerable)new RewriteQueryEnumerable<Dummy>(enumerable)).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void GetEnumeratorTypedShouldReturnEnumerator()
        {
            var actual = new RewriteQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void DisposeShouldCallDispose()
        {
            new RewriteQueryEnumerable<Dummy>(enumerable).Dispose();

            Assert.True(enumerable.DisposeCalled);
        }
    }
}
