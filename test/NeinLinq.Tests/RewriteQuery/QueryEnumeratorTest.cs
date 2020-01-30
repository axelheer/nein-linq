using System;
using System.Collections;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class QueryEnumeratorTest
    {
        private readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void LegacyCurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = ((IEnumerator)new RewriteQueryEnumerator<Dummy>(enumerator)).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public void CurrentShouldReturnCurrent()
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
        public void ResetShouldMoveNext()
        {
            new RewriteQueryEnumerator<Dummy>(enumerator).Reset();

            Assert.True(enumerator.ResetCalled);
        }

        [Fact]
        public async Task MoveNextAsyncShouldMoveNext()
        {
            await new RewriteQueryEnumerator<Dummy>(enumerator).MoveNextAsync();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }

        [Fact]
        public async Task DisposeAsyncShouldDispose()
        {
            await new RewriteQueryEnumerator<Dummy>(enumerator).DisposeAsync();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
