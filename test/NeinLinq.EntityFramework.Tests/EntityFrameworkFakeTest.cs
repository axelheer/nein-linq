using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Xunit;

namespace NeinLinq.Tests;

public class EntityFrameworkFakeTest
{
    [Fact]
    public async Task ToListAsync_Throws()
    {
        var query = CreateQuery();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(()
            => query.ToListAsync()
        );

        Assert.Matches(@"Only sources that implement '?IDbAsyncEnumerable'? can be used for Entity Framework asynchronous operations\.", error.Message);
    }

    [Fact]
    public async Task ToListAsync_WithRewrite_ListsAsync()
    {
        var rewriter = new TestExpressionVisitor();
        var query = CreateQuery().DbRewrite(rewriter);

        var result = await query.ToListAsync();

        Assert.True(rewriter.VisitCalled);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task SumAsync_Throws()
    {
        var query = CreateQuery();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(()
            => query.SumAsync(m => m.Number)
        );

        Assert.Matches(@"Only providers that implement '?IDbAsyncQueryProvider'? can be used for Entity Framework asynchronous operations\.", error.Message);
    }

    [Fact]
    public async Task SumAsync_WithRewrite_SumsAsync()
    {
        var rewriter = new TestExpressionVisitor();
        var query = CreateQuery().DbRewrite(rewriter);

        var result = await query.SumAsync(m => m.Number);

        Assert.True(rewriter.VisitCalled);
        Assert.Equal(194.48, result, 2);
    }

    [Fact]
    public async Task GetAsyncEnumerator_MovesNextAsync()
    {
        var rewriter = new TestExpressionVisitor();
        var query = CreateQuery().DbRewrite(rewriter);

        var result = await ((IDbAsyncEnumerable)query).GetAsyncEnumerator().MoveNextAsync(default);

        Assert.True(rewriter.VisitCalled);
        Assert.True(result);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesAsync()
    {
        var rewriter = new TestExpressionVisitor();
        var query = CreateQuery().DbRewrite(rewriter);

        var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(
            Expression.Call(typeof(Queryable), nameof(Queryable.Count), [typeof(Model)], query.Expression),
            default
        );

        Assert.True(rewriter.VisitCalled);
        Assert.Equal(3, (int)result);
    }

    private static IQueryable<Model> CreateQuery()
    {
        var data = new[]
        {
            new Model
            {
                Id = 1,
                Name = "Asdf",
                Number = 123.45f
            },
            new Model
            {
                Id = 2,
                Name = "Qwer",
                Number = 67.89f
            },
            new Model
            {
                Id = 3,
                Name = "Narf",
                Number = 3.14f
            }
        };

        return data.AsQueryable();
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public float Number { get; set; }
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
}
