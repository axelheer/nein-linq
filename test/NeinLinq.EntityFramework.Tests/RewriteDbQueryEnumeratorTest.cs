using System.Data.Entity.Infrastructure;
using Xunit;

namespace NeinLinq.Tests;

public class RewriteDbQueryEnumeratorTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var enumeratorError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryEnumerator<Model>(null!));

        Assert.Equal("enumerator", enumeratorError.ParamName);
    }

    [Fact]
    public void TypedCurrent_ReturnsCurrent()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        Assert.Equal(enumerator.Current, subject.Current);
    }

    [Fact]
    public void UntypedCurrent_ReturnsCurrent()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        Assert.Equal(enumerator.Current, ((IEnumerator)subject).Current);
    }

    [Fact]
    public void AsyncCurrent_ReturnsCurrent()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        Assert.Equal(enumerator.Current, ((IDbAsyncEnumerator)subject).Current);
    }

    [Fact]
    public void MoveNext_MovesNext()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        _ = subject.MoveNext();

        Assert.True(enumerator.MoveNextCalled);
    }

    [Fact]
    public void Reset_Resets()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        subject.Reset();

        Assert.True(enumerator.ResetCalled);
    }

    [Fact]
    public async Task MoveNextAsync_MovesNextAsync()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        _ = await subject.MoveNextAsync(default);

        Assert.True(enumerator.MoveNextCalled);
    }

    [Fact]
    public void Dispose_Disposes()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        subject.Dispose();

        Assert.True(enumerator.DisposeCalled);
    }

    [Fact]
    public async Task DisposeAsync_DisposesAsync()
    {
        using var enumerator = new TestEnumerator();
        using var subject = new RewriteDbQueryEnumerator<Model>(enumerator);

        await subject.DisposeAsync();

        Assert.True(enumerator.DisposeCalled);
    }

    private class Model
    {
    }

    private sealed class TestEnumerator : IEnumerator<Model>
    {
        public Model Current { get; set; }
            = new Model();

        object IEnumerator.Current => Current;

        public bool DisposeCalled { get; set; }

        public void Dispose()
            => DisposeCalled = true;

        public bool MoveNextCalled { get; set; }

        public bool MoveNext()
        {
            MoveNextCalled = true;
            return !MoveNextCalled;
        }

        public bool ResetCalled { get; set; }

        public void Reset()
            => ResetCalled = true;
    }
}
