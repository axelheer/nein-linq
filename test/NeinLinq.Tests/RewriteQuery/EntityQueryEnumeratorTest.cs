using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.EntityFrameworkCore;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryEnumeratorTest
    {
        readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void CurrentUntypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = ((RewriteEntityQueryEnumerator)new RewriteEntityQueryEnumerator<Dummy>(enumerator)).Current;

            Assert.Equal(enumerator.Current, (Dummy)actual);
        }

        [Fact]
        public void CurrentTypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteEntityQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public void MoveNextShouldMoveNext()
        {
            new RewriteEntityQueryEnumerator<Dummy>(enumerator).MoveNext();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void ResetShouldReset()
        {
            new RewriteEntityQueryEnumerator<Dummy>(enumerator).Reset();

            Assert.True(enumerator.ResetCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteEntityQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
