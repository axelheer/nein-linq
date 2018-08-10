using System;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.EntityFramework;
using NeinLinq.Fakes.RewriteQuery;
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

            var actual = ((IDbAsyncEnumerator)new RewriteDbQueryEnumerator<Dummy>(enumerator)).Current;

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
        public async Task MoveNextShouldMoveNext()
        {
            await new RewriteDbQueryEnumerator<Dummy>(enumerator).MoveNextAsync(CancellationToken.None);

            Assert.True(enumerator.MoveNextCalled);
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            new RewriteDbQueryEnumerator<Dummy>(enumerator).Dispose();

            Assert.True(enumerator.DisposeCalled);
        }
    }
}
