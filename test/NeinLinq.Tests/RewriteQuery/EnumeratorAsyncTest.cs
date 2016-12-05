#if IX || EFCORE

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EnumeratorAsyncTest
    {
        readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteAsyncQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void CurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteAsyncQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public async Task MoveNextShouldMoveNext()
        {
            await new RewriteAsyncQueryEnumerator<Dummy>(enumerator).MoveNext(CancellationToken.None);

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteAsyncQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}

#endif
