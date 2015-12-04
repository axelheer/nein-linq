using NeinLinq.Tests.DbAsyncQueryData;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Xunit;

namespace NeinLinq.Tests
{
    public class DbAsyncQueryTest : IDisposable
    {
        readonly Context db;

        public DbAsyncQueryTest()
        {
            db = new Context();

            db.Dummies.RemoveRange(db.Dummies);
            db.SaveChanges();

            db.Dummies.AddRange(new[]
            {
                new Dummy
                {
                    Id = 1,
                    Name = "Asdf",
                    Number = 123.45m
                },
                new Dummy
                {
                    Id = 2,
                    Name = "Qwer",
                    Number = 67.89m
                },
                new Dummy
                {
                    Id = 3,
                    Name = "Narf",
                    Number = 3.14m
                }
            });
            db.SaveChanges();
        }

        [Fact]
        public async void ToListAsyncShouldWork()
        {
            await db.Dummies.ToListAsync();
        }

        [Fact]
        public async void ToListAsyncShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new Rewriter());

            var result = await query.ToListAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async void SumAsyncShouldWork()
        {
            await db.Dummies.SumAsync(d => d.Number);
        }

        [Fact]
        public async void SumAsyncShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new Rewriter());

            var result = await query.SumAsync(d => d.Number);

            Assert.Equal(194.48m, result, 2);
        }

        [Fact]
        public async void UntypedEnumeratorShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new Rewriter());

            var enumerator = ((IDbAsyncEnumerable)query).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync(CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async void UntypedExecuteShouldSucceed()
        {
            var query = db.Dummies.Rewrite(new Rewriter());

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(expression, CancellationToken.None);

            Assert.Equal(3, (int)result);
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
