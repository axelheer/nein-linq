using System;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public class EntityQueryEnumeratorTest
    {
        private readonly DummyEnumerator enumerator = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new RewriteEntityQueryEnumerator<Dummy>(null));
        }

        [Fact]
        public void CurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteEntityQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public async Task MoveNextShouldMoveNext()
        {
            await new RewriteEntityQueryEnumerator<Dummy>(enumerator).MoveNextAsync();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public async Task DisposeShouldDispose()
        {
            await new RewriteEntityQueryEnumerator<Dummy>(enumerator).DisposeAsync();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
