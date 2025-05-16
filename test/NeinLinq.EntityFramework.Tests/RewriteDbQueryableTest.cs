using System.Data.Entity.Infrastructure;
using Xunit;

namespace NeinLinq.Tests;

public class RewriteDbQueryableTest
{
    [Fact]
    public void Ctor_NullArgument_Throws()
    {
        var query = CreateQuery();
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteDbQueryProvider(query.Provider, rewriter);

        var queryError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryable<Model>(null!, provider));
        var providerError = Assert.Throws<ArgumentNullException>(()
            => new RewriteDbQueryable<Model>(query, null!));

        Assert.Equal("query", queryError.ParamName);
        Assert.Equal("provider", providerError.ParamName);
    }

    [Fact]
    public void TypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = subject.GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedGetEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = ((IEnumerable)subject).GetEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void TypedGetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = subject.GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void UntypedGetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = ((IDbAsyncEnumerable)subject).GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public void ExplicitImplementation_TypedGetAsyncEnumerator_RewritesQuery()
    {
        var query = CreateQuery();

        var (rewriter, _, subject) = CreateRewriteQuery(query);

        _ = ((IAsyncEnumerable<Model>)subject).GetAsyncEnumerator();

        Assert.True(rewriter.VisitCalled);
    }

    [Fact]
    public async Task ExplicitImplementation_TypedGetAsyncEnumerator_Uses_DbAsyncEnumerable()
    {
        var moveNextAsyncCalled = false;

        var query = CreateDbAsyncEnumerableQuery(() => moveNextAsyncCalled = true);

        var (_, _, subject) = CreateRewriteQuery(query);

        await using var asyncEnumerator = ((IAsyncEnumerable<Model>)subject).GetAsyncEnumerator();

        await asyncEnumerator.MoveNextAsync();

        Assert.True(moveNextAsyncCalled);
    }

    [Fact]
    public void ElementType_ReturnsElementType()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query.ElementType, subject.ElementType);
    }

    [Fact]
    public void Expression_ReturnsExpression()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query.Expression, subject.Expression);
    }

    [Fact]
    public void Provider_ReturnsProvider()
    {
        var query = CreateQuery();

        var (_, provider, subject) = CreateRewriteQuery(query);

        Assert.Equal(provider, subject.Provider);
    }

    [Fact]
    public void QueryableProvider_ReturnsProvider()
    {
        var query = CreateQuery();

        var (_, provider, subject) = CreateRewriteQuery(query);

        Assert.Equal(provider, ((IQueryable)subject).Provider);
    }

    [Fact]
    public void Query_ReturnsQuery()
    {
        var query = CreateQuery();

        var (_, _, subject) = CreateRewriteQuery(query);

        Assert.Equal(query, subject.Query);
    }

    private static (TestExpressionVisitor, RewriteDbQueryProvider, RewriteDbQueryable<Model>) CreateRewriteQuery(IQueryable<Model> query)
    {
        var rewriter = new TestExpressionVisitor();
        var provider = new RewriteDbQueryProvider(query.Provider, rewriter);

        return (rewriter, provider, new RewriteDbQueryable<Model>(query, provider));
    }

    private static IQueryable<Model> CreateQuery() => Enumerable.Empty<Model>().AsQueryable();

#pragma warning disable CA1859
    private static IQueryable<Model> CreateDbAsyncEnumerableQuery(Action moveNextAsyncCalled) => new TestDbAsyncEnumerable<Model>([], moveNextAsyncCalled);
#pragma warning restore CA1859

    private class Model
    {
    }

    private class TestExpressionVisitor : ExpressionVisitor
    {
        public bool VisitCalled { get; set; }

        public override Expression? Visit(Expression? node)
        {
            VisitCalled = true;
            return base.Visit(node);
        }
    }

    private sealed class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly Action moveNextAsyncCalled;

        public TestDbAsyncEnumerable(IEnumerable<T> enumerable, Action moveNextAsyncCalled) : base(enumerable)
        {
            this.moveNextAsyncCalled = moveNextAsyncCalled;
        }

        public TestDbAsyncEnumerable(Expression expression, Action moveNextAsyncCalled) : base(expression)
        {
            this.moveNextAsyncCalled = moveNextAsyncCalled;
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() => throw new NotSupportedException();

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator(), moveNextAsyncCalled);

        IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this, moveNextAsyncCalled);

        private sealed class TestDbAsyncEnumerator<T1> : IDbAsyncEnumerator<T1>
        {
            private readonly IEnumerator<T1> inner;
            private readonly Action moveNextAsyncCalled;

            public TestDbAsyncEnumerator(IEnumerator<T1> inner, Action moveNextAsyncCalled)
            {
                this.inner = inner;
                this.moveNextAsyncCalled = moveNextAsyncCalled;
            }

            public void Dispose() => inner.Dispose();

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                moveNextAsyncCalled();
                return Task.FromResult(inner.MoveNext());
            }

            public T1 Current => inner.Current;

            object IDbAsyncEnumerator.Current => Current!;
        }

        private sealed class TestDbAsyncQueryProvider<T2> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider inner;
            private readonly Action moveNextAsyncCalled;

            public TestDbAsyncQueryProvider(IQueryProvider inner, Action moveNextAsyncCalled)
            {
                this.inner = inner;
                this.moveNextAsyncCalled = moveNextAsyncCalled;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                var queryableType = typeof(TestDbAsyncEnumerable<>).MakeGenericType(expression.Type.GetGenericArguments()[0]);

                return (IQueryable)Activator.CreateInstance(queryableType, expression, moveNextAsyncCalled)!;
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestDbAsyncEnumerable<TElement>(expression, moveNextAsyncCalled);

            public object? Execute(Expression expression) => inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

            public Task<object?> ExecuteAsync(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute(expression));

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));
        }
    }
}
