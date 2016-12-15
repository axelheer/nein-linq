using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.EntityFrameworkCore;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryEnumerableTest
    {
        readonly DummyEnumerable enumerable = new DummyEnumerable();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryEnumerable<Dummy>(null));
        }

        [Fact]
        public void GetEnumeratorUntypedShouldReturnEnumerator()
        {
            var actual = ((RewriteEntityQueryEnumerable)new RewriteEntityQueryEnumerable<Dummy>(enumerable)).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void GetEnumeratorTypedShouldReturnEnumerator()
        {
            var actual = new RewriteEntityQueryEnumerable<Dummy>(enumerable).GetEnumerator();

            Assert.IsType<DummyEnumerator>(actual);
        }

        [Fact]
        public void DisposeShouldCallDispose()
        {
            new RewriteEntityQueryEnumerable<Dummy>(enumerable).Dispose();

            Assert.True(enumerable.DisposeCalled);
        }
    }
}
