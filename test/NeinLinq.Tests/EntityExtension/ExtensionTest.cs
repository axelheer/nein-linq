using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NeinLinq.Fakes.EntityExtension;
using Xunit;

namespace NeinLinq.Tests.EntityExtension
{
    public class ExtensionTest
    {
        [Fact]
        public void ShouldInject()
        {
            var services = new ServiceCollection();

            _ = services.AddDbContext<Context>(options =>
                options.UseSqlite("Data Source=NeinLinq.EntityExtension.db").WithLambdaInjection());

            using var serviceProvider = services.BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<Context>();

            _ = context.Database.EnsureDeleted();
            _ = context.Database.EnsureCreated();

            context.Dummies.AddRange(
                new Dummy { Name = "Heinz" },
                new Dummy { Name = "Narf" },
                new Dummy { Name = "Wat" }
            );

            _ = context.SaveChanges();

            var query = from d in context.Dummies.AsQueryable()
                        where d.IsNarf()
                        select d.Id;

            Assert.Equal(1, query.Count());
        }

        [Fact]
        public void ShouldNotInject()
        {
            var services = new ServiceCollection();

            _ = services.AddDbContext<Context>(options =>
                options.UseSqlite("Data Source=NeinLinq.EntityExtension.db"));

            using var serviceProvider = services.BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<Context>();

            _ = context.Database.EnsureDeleted();
            _ = context.Database.EnsureCreated();

            var query = from d in context.Dummies.AsQueryable()
                        where d.IsNarf()
                        select d.Id;

            _ = Assert.Throws<InvalidOperationException>(() => query.Count());
        }
    }
}
