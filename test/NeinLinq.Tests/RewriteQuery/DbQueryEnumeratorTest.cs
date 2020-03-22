using System;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public sealed class DbQueryEnumeratorTest : IDisposable
    {
        private readonly DummyEnumerator enumerator
            = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(()
                => new RewriteDbQueryEnumerator<Dummy>(null!));
        }

        [Fact]
        public void CurrentUntypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            using (var subject = new RewriteDbQueryEnumerator<Dummy>(enumerator))
            {
                var actual = ((IDbAsyncEnumerator)subject).Current;

                Assert.Equal(enumerator.Current, (Dummy)actual);
            }
        }

        [Fact]
        public void CurrentTypedShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            using (var subject = new RewriteDbQueryEnumerator<Dummy>(enumerator))
            {
                var actual = subject.Current;

                Assert.Equal(enumerator.Current, actual);
            }
        }

        [Fact]
        public async Task MoveNextShouldMoveNext()
        {
            using (var subject = new RewriteDbQueryEnumerator<Dummy>(enumerator))
            {
                _ = await subject.MoveNextAsync(CancellationToken.None);

                Assert.True(enumerator.MoveNextCalled);
            }
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            using (var subject = new RewriteDbQueryEnumerator<Dummy>(enumerator))
            {
                subject.Dispose();

                Assert.True(enumerator.DisposeCalled);
            }
        }

        public void Dispose()
            => enumerator.Dispose();
    }
}
