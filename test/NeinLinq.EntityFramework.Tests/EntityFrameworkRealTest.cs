using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using Xunit;

namespace NeinLinq.Tests;

public class EntityFrameworkRealTest
{
    [Fact]
    public void DebugString_ShowsSql()
    {
        using var context = CreateContext();
        var query = context.Models.ToDbInjectable();

        var actual = ((RewriteQueryable<Model>)query).DebugString;

        Assert.Matches("SELECT", actual);
    }

    [Fact]
    public async Task AsNoTracking_MarksQuery()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await query.AsNoTracking().ToListAsync();

        Assert.True(rewriter.VisitCalled);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Include_MarksQuery()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await query.Include(d => d.Other).ToListAsync();

        Assert.True(rewriter.VisitCalled);
        Assert.All(result, r => Assert.Equal(r.Name, r.Other?.Name));
    }

    [Fact]
    public async Task SubQuery_ResolvesQuery()
    {
        using var context = CreateContext();
        var innerRewriter = new TestExpressionVisitor();
        var innerQuery = context.Models.DbRewrite(innerRewriter);
        var outerRewriter = new TestExpressionVisitor();
        var outerQuery = context.Models.DbRewrite(outerRewriter)
            .Where(m => innerQuery.Any(n => n.Id < m.Id));

        var result = await outerQuery.ToListAsync();

        Assert.True(outerRewriter.VisitCalled);
        Assert.True(innerRewriter.VisitCalled);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SubQuery_WithProjection_ResolvesQuery()
    {
        using var context = CreateContext();
        var innerRewriter = new TestExpressionVisitor();
        var innerQuery = context.Models.DbRewrite(innerRewriter)
            .Select(m => new { m.Id });
        var outerRewriter = new TestExpressionVisitor();
        var outerQuery = context.Models.DbRewrite(outerRewriter)
            .Where(m => innerQuery.Any(n => n.Id < m.Id));

        var result = await outerQuery.ToListAsync();

        Assert.True(outerRewriter.VisitCalled);
        Assert.True(innerRewriter.VisitCalled);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SubQuery_WithInclude_ResolvesQuery()
    {
        using var context = CreateContext();
        var innerRewriter = new TestExpressionVisitor();
        var innerQuery = context.Models.DbRewrite(innerRewriter);
        var outerRewriter = new TestExpressionVisitor();
        var outerQuery = context.Models.DbRewrite(outerRewriter)
            .Where(m => innerQuery.Any(n => n.Id < m.Id))
            .Include(m => m.Other);

        var result = await outerQuery.ToListAsync();

        Assert.All(result, r => Assert.Equal(r.Name, r.Other?.Name));
        Assert.True(outerRewriter.VisitCalled);
        Assert.True(innerRewriter.VisitCalled);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SubQuery_WithAsNoTracking_ResolvesQuery()
    {
        using var context = CreateContext();
        var innerRewriter = new TestExpressionVisitor();
        var innerQuery = context.Models.DbRewrite(innerRewriter);
        var outerRewriter = new TestExpressionVisitor();
        var outerQuery = context.Models.DbRewrite(outerRewriter)
            .Where(m => innerQuery.Any(n => n.Id < m.Id))
            .AsNoTracking();

        var result = await outerQuery.ToListAsync();

        Assert.True(outerRewriter.VisitCalled);
        Assert.True(innerRewriter.VisitCalled);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ToListAsync_ListsAsync()
    {
        using var context = CreateContext();

        var result = await context.Models.ToListAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ToListAsync_WithRewrite_ListsAsync()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await query.ToListAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task SumAsync_SumsAsync()
    {
        using var context = CreateContext();

        var result = await context.Models.SumAsync(m => m.Number);

        Assert.Equal(194.48, result, 2);
    }

    [Fact]
    public async Task SumAsync_WithRewrite_SumsAsync()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await query.SumAsync(m => m.Number);

        Assert.Equal(194.48, result, 2);
    }

    [Fact]
    public async Task GetAsyncEnumerator_MovesNextAsync()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await ((IDbAsyncEnumerable)query).GetAsyncEnumerator().MoveNextAsync(default);

        Assert.True(rewriter.VisitCalled);
        Assert.True(result);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesAsync()
    {
        using var context = CreateContext();
        var rewriter = new TestExpressionVisitor();
        var query = context.Models.DbRewrite(rewriter);

        var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(
            Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Model) }, query.Expression),
            default
        );

        Assert.True(rewriter.VisitCalled);
        Assert.Equal(3, (int)result);
    }

    private static TestContext CreateContext()
    {
        using var init = new TestContext();

        init.CreateDatabase(
            new Model
            {
                Name = "Asdf",
                Number = 123.45f,
                Other = new OtherModel
                {
                    Name = "Asdf"
                }
            },
            new Model
            {
                Name = "Qwer",
                Number = 67.89f,
                Other = new OtherModel
                {
                    Name = "Qwer"
                }
            },
            new Model
            {
                Name = "Narf",
                Number = 3.14f,
                Other = new OtherModel
                {
                    Name = "Narf"
                }
            }
        );

        return new TestContext();
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public float Number { get; set; }

        public int OtherId { get; set; }

        public OtherModel? Other { get; set; }
    }

    private class OtherModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    private class TestContext : DbContext
    {
        public DbSet<Model> Models { get; }

        public TestContext() : base("Data Source=EntityFrameworkRealTest.db")
        {
            Models = Set<Model>();
        }

        public void CreateDatabase(params Model[] seed)
        {
            if (File.Exists("EntityFrameworkRealTest.db"))
                return;

            File.Create("EntityFrameworkRealTest.db").Close();

            _ = Database.ExecuteSqlCommand(@"
                    CREATE TABLE Models (
                        Id INTEGER PRIMARY KEY NOT NULL,
                        Name TEXT,
                        Number REAL NOT NULL,
                        OtherId INTEGER NOT NULL
                    )"
            );

            _ = Database.ExecuteSqlCommand(@"
                    CREATE TABLE OtherModels (
                        Id INTEGER PRIMARY KEY  NOT NULL,
                        Name TEXT
                    )"
            );

            _ = Models.AddRange(seed);
            _ = SaveChanges();
        }
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
