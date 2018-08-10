using System;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.EntityFrameworkCore;
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
            await new RewriteEntityQueryEnumerator<Dummy>(enumerator).MoveNext(CancellationToken.None);

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteEntityQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
