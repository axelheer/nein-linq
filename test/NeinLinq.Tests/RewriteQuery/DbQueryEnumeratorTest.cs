#if NET461

using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.EntityFramework;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class DbQueryEnumeratorTest
    {
        readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteDbQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void CurrentUntypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = ((RewriteDbQueryEnumerator)new RewriteDbQueryEnumerator<Dummy>(enumerator)).Current;

            Assert.Equal(enumerator.Current, (Dummy)actual);
        }

        [Fact]
        public void CurrentTypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteDbQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public void MoveNextShouldMoveNext()
        {
            new RewriteDbQueryEnumerator<Dummy>(enumerator).MoveNext();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void ResetShouldReset()
        {
            new RewriteDbQueryEnumerator<Dummy>(enumerator).Reset();

            Assert.True(enumerator.ResetCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteDbQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}

#endif
