using System;
using System.Collections;
using System.Threading.Tasks;
using NeinLinq.Fakes.RewriteQuery;
using Xunit;

namespace NeinLinq.Tests.RewriteQuery
{
    public sealed class QueryEnumeratorTest : IDisposable
    {
        private readonly DummyEnumerator enumerator
            = new DummyEnumerator();

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(()
                => new RewriteQueryEnumerator<Dummy>(null!));
        }

        [Fact]
        public void LegacyCurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                var actual = ((IEnumerator)subject).Current;

                Assert.Equal(enumerator.Current, actual);
            }
        }

        [Fact]
        public void CurrentShouldReturnCurrent()
        {
            enumerator.Current = new Dummy();

            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                var actual = subject.Current;

                Assert.Equal(enumerator.Current, actual);
            }
        }

        [Fact]
        public void MoveNextShouldMoveNext()
        {
            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                _ = subject.MoveNext();

                Assert.True(enumerator.MoveNextCalled);
            }
        }

        [Fact]
        public void ResetShouldMoveNext()
        {
            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                subject.Reset();

                Assert.True(enumerator.ResetCalled);
            }
        }

        [Fact]
        public async Task MoveNextAsyncShouldMoveNext()
        {
            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                _ = await subject.MoveNextAsync();

                Assert.True(enumerator.MoveNextCalled);
            }
        }

        [Fact]
        public void DisposeShouldDispose()
        {
            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                subject.Dispose();

                Assert.True(enumerator.DisposeCalled);
            }
        }

        [Fact]
        public async Task DisposeAsyncShouldDispose()
        {
            using (var subject = new RewriteQueryEnumerator<Dummy>(enumerator))
            {
                await subject.DisposeAsync();

                Assert.True(enumerator.DisposeCalled);
            }
        }

        public void Dispose()
            => enumerator.Dispose();
    }
}
