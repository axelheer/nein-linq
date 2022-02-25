using Xunit;

namespace NeinLinq.Tests;

public class RewriteQueryEnumerableTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var enumerableError = Assert.Throws<ArgumentNullException>(()
            => new RewriteQueryEnumerable<Model>(null!));

        Assert.Equal("enumerable", enumerableError.ParamName);
    }

    [Fact]
    public void TypedGetEnumerator_GetsEnumerator()
    {
        var enumerable = new TestEnumerable();
        var subject = new RewriteQueryEnumerable<Model>(enumerable);

        _ = Assert.IsType<RewriteQueryEnumerator<Model>>(subject.GetEnumerator());
    }

    [Fact]
    public void UntypedGetEnumerator_GetsEnumerator()
    {
        var enumerable = new TestEnumerable();
        var subject = new RewriteQueryEnumerable<Model>(enumerable);

        _ = Assert.IsType<RewriteQueryEnumerator<Model>>(((IEnumerable)subject).GetEnumerator());
    }

    [Fact]
    public void GetAsyncEnumerator_GetsAsyncEnumerator()
    {
        var enumerable = new TestEnumerable();
        var subject = new RewriteQueryEnumerable<Model>(enumerable);

        _ = Assert.IsType<RewriteQueryEnumerator<Model>>(subject.GetAsyncEnumerator());
    }

    private class Model
    {
    }

    private class TestEnumerable : IEnumerable<Model>
    {
        public IEnumerator<Model> GetEnumerator()
            => new TestEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    private sealed class TestEnumerator : IEnumerator<Model>
    {
        public Model Current
            => throw new NotImplementedException();

        object IEnumerator.Current
            => throw new NotImplementedException();

        public void Dispose()
            => throw new NotImplementedException();

        public bool MoveNext()
            => throw new NotImplementedException();

        public void Reset()
            => throw new NotImplementedException();
    }
}
