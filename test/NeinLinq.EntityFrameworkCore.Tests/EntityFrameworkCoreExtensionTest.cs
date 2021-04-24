
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NeinLinq.Tests
{
    public class EntityFrameworkCoreExtensionTest
    {
        [Fact]
        public void Ctor_NullArgument_Throws()
        {
            var optionsBuilderError = Assert.Throws<ArgumentNullException>(()
                => InjectableDbContextOptionsBuilderExtensions.WithLambdaInjection(null!));
            var greenlistError = Assert.Throws<ArgumentNullException>(()
                => InjectableDbContextOptionsBuilderExtensions.WithLambdaInjection(new DbContextOptionsBuilder(), null!));

            Assert.Equal("optionsBuilder", optionsBuilderError.ParamName);
            Assert.Equal("greenlist", greenlistError.ParamName);
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
        public void Query_Throws()
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
        public void SubQuery_WithLambdaInjection_ResolvesLambdaInjection()
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
                             from n in innerQuery
                             select m.Id;

            Assert.Equal(3, outerQuery.Count());
        }

#pragma warning disable CA1812

        private class Model
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public bool IsNarf
                => throw new InvalidOperationException($"Unable to determine, whether {Name} is Narf or not.");

            public static Expression<Func<Model, bool>> IsNarfExpr
                => d => d.Name == "Narf";
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
    }
}
