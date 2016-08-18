using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeinLinq.Tests.DbAsyncQueryData;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NeinLinq.Tests
{
    public class DbAsyncTest : IDisposable
    {
        readonly Context db;

        public DbAsyncTest()
        {
            db = new Context();

            db.Database.EnsureCreated();
            db.Dummies.RemoveRange(db.Dummies);
            db.SaveChanges();

            db.Dummies.AddRange(new[]
            {
                new Dummy
                {
                    Name = "Asdf",
                    Number = 123.45m
                },
                new Dummy
                {
                    Name = "Qwer",
                    Number = 67.89m
                },
                new Dummy
                {
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
            var rewriter = new Rewriter();
            var query = db.Dummies.Rewrite(rewriter);

            var result = await query.ToListAsync();

            Assert.True(rewriter.VisitCalled);
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
            var rewriter = new Rewriter();
            var query = db.Dummies.Rewrite(rewriter);

            var result = await query.SumAsync(d => d.Number);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(194.48m, result, 2);
        }

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.Rewrite(rewriter);

            var enumerator = ((IAsyncQueryProvider)query.Provider).ExecuteAsync<Dummy>(query.Expression).GetEnumerator();

            var result = await enumerator.MoveNext(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
