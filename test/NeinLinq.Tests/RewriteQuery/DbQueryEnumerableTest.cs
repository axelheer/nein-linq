using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.EntityFramework;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryEnumerableTest
    {
        readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldReturnEnumerator()
        {
            var actual = ((RewriteDbQueryEnumerable)new RewriteDbQueryEnumerable<Dummy>(enumerable)).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void GetEnumeratorTypedShouldReturnEnumerator()
        {
            var actual = new RewriteDbQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void DisposeShouldCallDispose()
        {
            new RewriteDbQueryEnumerable<Dummy>(enumerable).Dispose();

            Assert.True(enumerable.DisposeCalled);
        }
    }
}
