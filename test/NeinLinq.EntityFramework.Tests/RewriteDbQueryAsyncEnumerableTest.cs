using System.Data.Entity.Infrastructure;
using Xunit;

namespace NeinLinq.Tests;

public class RewriteDbQueryAsyncEnumerableTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var dbAsyncEnumerableError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryAsyncEnumerable<string>(null!));

        Assert.Equal("dbAsyncEnumerable", dbAsyncEnumerableError.ParamName);
    }

    [Fact]
    public async Task AsyncEnumerator_MovesNextAsync()
    {
        var dbAsyncEnumerator = new TestDbAsyncEnumerator();
        var dbAsyncEnumerable = new TestDbAsyncEnumerable(dbAsyncEnumerator);

        var asyncEnumerable = new RewriteDbQueryAsyncEnumerable<string>(dbAsyncEnumerable);

        await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(default);
        var moved = await asyncEnumerator.MoveNextAsync();

        Assert.Multiple(() =>
        {
            Assert.True(moved);
            Assert.True(dbAsyncEnumerator.MoveNextAsyncCalled);
        });
    }

    [Fact]
    public async Task AsyncEnumerator_Current_Returns()
    {
        var asyncEnumerable = new RewriteDbQueryAsyncEnumerable<string>(new TestDbAsyncEnumerable(new TestDbAsyncEnumerator()));

        await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(default);
        await asyncEnumerator.MoveNextAsync();

        Assert.Equal("Current", asyncEnumerator.Current);
    }

    [Fact]
    public async Task AsyncEnumerator_Disposes_DbAsyncEnumerator()
    {
        var dbAsyncEnumerator = new TestDbAsyncEnumerator();
        var dbAsyncEnumerable = new TestDbAsyncEnumerable(dbAsyncEnumerator);

        var asyncEnumerable = new RewriteDbQueryAsyncEnumerable<string>(dbAsyncEnumerable);

        await using (var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(default))
        {
            await asyncEnumerator.MoveNextAsync();
        }

        Assert.True(dbAsyncEnumerator.DisposeCalled);
    }

    [Fact]
    public async Task AsyncEnumerator_Cancels()
    {
        var dbAsyncEnumerator = new TestDbAsyncEnumerator();
        var dbAsyncEnumerable = new TestDbAsyncEnumerable(dbAsyncEnumerator);

        var asyncEnumerable = new RewriteDbQueryAsyncEnumerable<string>(dbAsyncEnumerable);

        using var cts = new CancellationTokenSource();

        await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(cts.Token);
        await asyncEnumerator.MoveNextAsync();

#pragma warning disable CA1849
        cts.Cancel();
#pragma warning restore CA1849

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await asyncEnumerator.MoveNextAsync();
        });
    }

    private sealed class TestDbAsyncEnumerable : IDbAsyncEnumerable
    {
        private readonly TestDbAsyncEnumerator dbAsyncEnumerator;

        public TestDbAsyncEnumerable(TestDbAsyncEnumerator dbAsyncEnumerator)
        {
            this.dbAsyncEnumerator = dbAsyncEnumerator;
        }

        public IDbAsyncEnumerator GetAsyncEnumerator() => dbAsyncEnumerator;
    }

    private sealed class TestDbAsyncEnumerator : IDbAsyncEnumerator
    {
        public bool MoveNextAsyncCalled { get; private set; }

        public bool DisposeCalled { get; private set; }

        public object? Current { get; private set; }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (MoveNextAsyncCalled) return Task.FromResult(false);

            Current = "Current";
            MoveNextAsyncCalled = true;

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }
}
