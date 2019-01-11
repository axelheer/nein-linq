﻿#if NETFRAMEWORK

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NeinLinq.Fakes.DbAsyncQuery;
using Xunit;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class RealTest : IDisposable
    {
        readonly Context db;

        public RealTest()
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

        [WindowsFact]
        public async Task SubQueryShouldSucceed()
        {
            var innerRewriter = new Rewriter();
            var outerRewriter = new Rewriter();

            var dummies = db.Dummies.DbRewrite(innerRewriter);

            var query = from dummy in db.Dummies.DbRewrite(outerRewriter)
                        where dummies.Any(d => d.Id < dummy.Id)
                        select dummy;

            var result = await query.ToListAsync();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
            Assert.Equal(2, result.Count);
        }

        [WindowsFact]
        public async Task ToListAsyncShouldWork()
        {
            await db.Dummies.ToListAsync();
        }

        [WindowsFact]
        public async Task ToListAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var result = await query.ToListAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, result.Count);
        }

        [WindowsFact]
        public async Task SumAsyncShouldWork()
        {
            await db.Dummies.SumAsync(d => d.Number);
        }

        [WindowsFact]
        public async Task SumAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var result = await query.SumAsync(d => d.Number);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(194.48m, result, 2);
        }

        [WindowsFact]
        public async Task AsyncEnumeratorShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var enumerator = ((IDbAsyncEnumerable)query).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

        [WindowsFact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(expression, CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
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

#endif
