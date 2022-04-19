
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NeinLinq.Tests;

public class EntityFrameworkCoreExtensionTest
{
    [Fact]
    public void WithRewriter_NullArgument_Throws()
    {
        var optionsBuilderError = Assert.Throws<ArgumentNullException>(()
            => RewriteDbContextOptionsBuilderExtensions.WithRewriter(null!, new Rewriter()));
        var rewriterError = Assert.Throws<ArgumentNullException>(()
            => RewriteDbContextOptionsBuilderExtensions.WithRewriter(new DbContextOptionsBuilder(), null!));

        Assert.Equal("optionsBuilder", optionsBuilderError.ParamName);
        Assert.Equal("rewriter", rewriterError.ParamName);
    }

    [Fact]
    public void WithRewriter_PopulatesInfo()
    {
        var info = new DbContextOptionsBuilder().WithRewriter(new Rewriter()).Options.FindExtension<RewriteDbContextOptionsExtension>()?.Info;
        var debugInfo = new Dictionary<string, string>();
        info?.PopulateDebugInfo(debugInfo);

        Assert.NotNull(info);
        Assert.False(info!.IsDatabaseProvider);
        Assert.Equal("Rewriter=NeinLinq.Tests.EntityFrameworkCoreExtensionTest+Rewriter", info.LogFragment);
        Assert.Equal(0, info.GetServiceProviderHashCode());
        Assert.Equal(new Dictionary<string, string>()
        {
            ["RewriteQuery:Rewriter:0:Type"] = "NeinLinq.Tests.EntityFrameworkCoreExtensionTest+Rewriter",
            ["RewriteQuery:Rewriter:0:Info"] = "I'm a rewriter."
        },
            debugInfo
        );
    }

    [Fact]
    public void Query_WithLambdaInjection_ResolvesLambdaInjection()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestContext>(options =>
            options.UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db").WithLambdaInjection(typeof(Model)));

        using var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<TestContext>();

        _ = context.Database.EnsureDeleted();
        _ = context.Database.EnsureCreated();

        context.Models.AddRange(
            new Model { Name = "Heinz" },
            new Model { Name = "Narf" },
            new Model { Name = "Wat" }
        );

        _ = context.SaveChanges();

        var query = from m in context.Models
                    where m.IsNarf
                    select m.Id;

        Assert.Equal(1, query.Count());
    }

    [Fact]
    public void Query_WithLambdaInjectionUsingOverride_ResolvesLambdaInjection()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestWithOverrideContext>(options =>
            options.UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db"));

        using var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<TestWithOverrideContext>();

        _ = context.Database.EnsureDeleted();
        _ = context.Database.EnsureCreated();

        context.Models.AddRange(
            new Model { Name = "Heinz" },
            new Model { Name = "Narf" },
            new Model { Name = "Wat" }
        );

        _ = context.SaveChanges();

        var query = from m in context.Models
                    where m.IsNarf
                    select m.Id;

        Assert.Equal(1, query.Count());
    }

    [Fact]
    public void Query_WithLambdaInjectionUsingWrongOrder_Throws()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestContext>(options =>
            options.WithLambdaInjection(typeof(Model)).UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db"));

        using var serviceProvider = services.BuildServiceProvider();

        _ = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<TestContext>());
    }

    [Fact]
    public void Query_WithoutLambdaInjection_Throws()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestContext>(options =>
            options.UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db"));

        using var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<TestContext>();

        _ = context.Database.EnsureDeleted();
        _ = context.Database.EnsureCreated();

        var query = from d in context.Models
                    where d.IsNarf
                    select d.Id;

        _ = Assert.Throws<InvalidOperationException>(() => query.Count());
    }

    [Fact]
    public void Query_WithInnerLambdaInjection_ResolvesLambdaInjection()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestContext>(options =>
            options.UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db").WithLambdaInjection(typeof(Model)));

        using var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<TestContext>();

        _ = context.Database.EnsureDeleted();
        _ = context.Database.EnsureCreated();

        context.Models.AddRange(
            new Model { Name = "Heinz" },
            new Model { Name = "Narf" },
            new Model { Name = "Wat" }
        );

        _ = context.SaveChanges();

        var innerQuery = context.Models.Where(m => m.IsNarf);

        var outerQuery = from m in context.Models
                         join n in innerQuery on m.Id equals n.Id
                         select n.Id;

        var query = context.Models.SelectMany(_ => outerQuery);

        Assert.Equal(3, query.Count());
    }

    [Fact]
    public void Query_WithNestedLambdaInjection_ResolvesLambdaInjection()
    {
        var services = new ServiceCollection();

        _ = services.AddDbContext<TestContext>(options =>
            options.UseSqlite("Data Source=EntityFrameworkCoreExtensionTest.db").WithLambdaInjection(typeof(Model)));

        using var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<TestContext>();

        _ = context.Database.EnsureDeleted();
        _ = context.Database.EnsureCreated();

        context.Models.AddRange(
            new Model
            {
                Name = "Heinz",
                Values =
                {
                        new ModelValue { Value = 1, IsActive = true },
                        new ModelValue { Value = 2 },
                        new ModelValue { Value = 3 }
                }
            },
            new Model
            {
                Name = "Narf",
                Values =
                {
                        new ModelValue { Value = 4 },
                        new ModelValue { Value = 5, IsActive = true },
                        new ModelValue { Value = 6 }
                }
            },
            new Model
            {
                Name = "Wat",
                Values =
                {
                        new ModelValue { Value = 7 },
                        new ModelValue { Value = 8 },
                        new ModelValue { Value = 9, IsActive = true }
                }
            }
        );

        _ = context.SaveChanges();

        var query = from m in context.Models
                    select new
                    {
                        m.Id,
                        m.Name,
                        m.ActiveValue
                    };

        Assert.Equal(15, query.Sum(m => m.ActiveValue));
    }

    private class Model
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public ISet<ModelValue> Values { get; set; }
            = new HashSet<ModelValue>();

        public bool IsNarf
            => throw new InvalidOperationException($"Unable to determine, whether {Name} is Narf or not.");

        public static Expression<Func<Model, bool>> IsNarfExpr
            => d => d.Name == "Narf";

        public int? ActiveValue
            => throw new InvalidOperationException($"Unable to determine, whether {Name} has active value or not.");

        public static Expression<Func<Model, int?>> ActiveValueExpr
            => d => d.Values.Where(v => v.IsActive == ModelValue.TrueValue).Select(v => v.Value).FirstOrDefault();
    }

    private class ModelValue
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public bool IsActive { get; set; }

        public static bool TrueValue
            => true;
    }

    private class TestContext : DbContext
    {
        public DbSet<Model> Models { get; }

        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
            Models = Set<Model>();
        }
    }

    private class TestWithOverrideContext : DbContext
    {
        public DbSet<Model> Models { get; }

        public TestWithOverrideContext(DbContextOptions<TestWithOverrideContext> options)
            : base(options)
        {
            Models = Set<Model>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.WithLambdaInjection(typeof(Model));
    }

    private class Rewriter : ExpressionVisitor
    {
        public override string ToString()
            => "I'm a rewriter.";
    }
}
