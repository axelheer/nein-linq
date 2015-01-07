using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Xunit;

namespace NeinLinq.Tests.EF6
{
    public class DbAsyncTest
    {
        static DbAsyncTest()
        {
            Database.SetInitializer<DbAsyncContext>(new DropCreateDatabaseAlways<DbAsyncContext>());
        }

        private readonly DbAsyncContext db;

        public DbAsyncTest()
        {
            db = new DbAsyncContext();
            db.Database.Initialize(force: true);

            db.Dummies.AddRange(new[] {
                new DbAsyncDummy
                {
                    Id = 1,
                    Name = "Asdf",
                    Number = 123.45m
                },
                new DbAsyncDummy
                {
                    Id = 2,
                    Name = "Qwer",
                    Number = 67.89m
                },
                new DbAsyncDummy
                {
                    Id = 3,
                    Name = "Narf",
                    Number = 3.14m
                }
            });
            db.SaveChanges();
        }

        [Fact]
        public async Task ToListAsyncShouldWork()
        {
            await db.Dummies.ToListAsync();
        }

        [Fact]
        public async Task ToListAsyncShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new FakeAsyncRewriter());

            var result = await query.ToListAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task SumAsyncShouldWork()
        {
            await db.Dummies.SumAsync(d => d.Number);
        }

        [Fact]
        public async Task SumAsyncShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new FakeAsyncRewriter());

            var result = await query.SumAsync(d => d.Number);

            Assert.Equal(194.48m, result, 2);
        }
    }
}
