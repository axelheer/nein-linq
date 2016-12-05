using System;

#if EF

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#elif EFCORE

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#endif

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Test : IDisposable
    {

#if EF || EFCORE

        readonly Context db;

        public Test()
        {
            db = new Context();
            db.ResetDatabase();

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

#endif

#if EF

        [Fact]
        public async Task AsyncEnumeratorShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.Rewrite(rewriter);

            var enumerator = ((IDbAsyncEnumerable)query).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.Rewrite(rewriter);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(expression, CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, (int)result);
        }

#elif EFCORE

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

#endif

        protected virtual void Dispose(bool disposing)
        {

#if EF || EFCORE

            if (disposing)
            {
                db.Dispose();
            }

#endif

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
