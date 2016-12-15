using NeinLinq.Fakes.RewriteQuery;
using NeinLinq.Queryable;
using System;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryEnumeratorTest
    {
        readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void CurrentUntypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = ((RewriteQueryEnumerator)new RewriteQueryEnumerator<Dummy>(enumerator)).Current;

            Assert.Equal(enumerator.Current, (Dummy)actual);
        }

        [Fact]
        public void CurrentTypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public void MoveNextShouldMoveNext()
        {
            new RewriteQueryEnumerator<Dummy>(enumerator).MoveNext();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void ResetShouldReset()
        {
            new RewriteQueryEnumerator<Dummy>(enumerator).Reset();

            Assert.True(enumerator.ResetCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
