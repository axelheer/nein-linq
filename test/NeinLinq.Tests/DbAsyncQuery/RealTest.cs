#if NETFRAMEWORK

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
        private readonly Context db;

        public RealTest()
        {
            using (var init = new Context())
            {
                init.ResetDatabase();

                init.Dummies.AddRange(new[]
                {
                    new Dummy
                    {
                        Name = "Asdf",
                        Number = 123.45m,
                        Other = new OtherDummy
                        {
                            Name = "Asdf"
                        }
                    },
                    new Dummy
                    {
                        Name = "Qwer",
                        Number = 67.89m,
                        Other = new OtherDummy
                        {
                            Name = "Qwer"
                        }
                    },
                    new Dummy
                    {
                        Name = "Narf",
                        Number = 3.14m,
                        Other = new OtherDummy
                        {
                            Name = "Narf"
                        }
                    }
                });
                init.SaveChanges();
            }

            db = new Context();
        }

        [WindowsFact]
        public async Task AsNoTrackingShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var result = await query.AsNoTracking().ToListAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, result.Count);
        }

        [WindowsFact]
        public async Task IncludeShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = db.Dummies.DbRewrite(rewriter);

            var result = await query.Include(d => d.Other).ToListAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.All(result, r => Assert.Equal(r.Name, r.Other.Name));
        }

        [WindowsFact]
        public async Task SubQueryShouldSucceed()
        {
            var innerRewriter = new Rewriter();
            var dummies = db.Dummies.DbRewrite(innerRewriter);

            var outerRewriter = new Rewriter();
            var query = from dummy in db.Dummies.DbRewrite(outerRewriter)
                        where dummies.Any(d => d.Id < dummy.Id)
                        select dummy;

            var result = await query.ToListAsync();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
            Assert.Equal(2, result.Count);
        }

        [WindowsFact]
        public async Task SubQueryShouldSucceedWithProjection()
        {
            var innerRewriter = new Rewriter();
            var dummies = db.Dummies.DbRewrite(innerRewriter).Select(d => new { d.Id });

            var outerRewriter = new Rewriter();
            var query = from dummy in db.Dummies.DbRewrite(outerRewriter)
                        where dummies.Any(d => d.Id < dummy.Id)
                        select dummy;

            var result = await query.ToListAsync();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
            Assert.Equal(2, result.Count);
        }

        [WindowsFact]
        public async Task SubQueryShouldSucceedWithInclude()
        {
            var innerRewriter = new Rewriter();
            var dummies = db.Dummies.DbRewrite(innerRewriter);

            var outerRewriter = new Rewriter();
            var query = from dummy in db.Dummies.DbRewrite(outerRewriter)
                        where dummies.Any(d => d.Id < dummy.Id)
                        select dummy;

            var result = await query.Include(d => d.Other).ToListAsync();

            Assert.True(outerRewriter.VisitCalled);
            Assert.True(innerRewriter.VisitCalled);
            Assert.All(result, r => Assert.Equal(r.Name, r.Other.Name));
            Assert.Equal(2, result.Count);
        }

        [WindowsFact]
        public async Task SubQueryShouldSucceedWithAsNoTracking()
        {
            var innerRewriter = new Rewriter();
            var dummies = db.Dummies.DbRewrite(innerRewriter);

            var outerRewriter = new Rewriter();
            var query = from dummy in db.Dummies.DbRewrite(outerRewriter)
                        where dummies.Any(d => d.Id < dummy.Id)
                        select dummy;

            var result = await query.AsNoTracking().ToListAsync();

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
