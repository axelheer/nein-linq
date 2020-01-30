using System;
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
        public void CurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            var actual = new RewriteQueryEnumerator<Dummy>(enumerator).Current;

            Assert.Equal(enumerator.Current, actual);
        }

        [Fact]
        public async Task MoveNextShouldMoveNext()
        {
            await new RewriteQueryEnumerator<Dummy>(enumerator).MoveNextAsync();

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public async Task DisposeShouldDispose()
        {
            await new RewriteQueryEnumerator<Dummy>(enumerator).DisposeAsync();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
