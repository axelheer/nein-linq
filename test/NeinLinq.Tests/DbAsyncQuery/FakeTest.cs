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
    public class FakeTest
    {
        private readonly IQueryable<Dummy> data;

        public FakeTest()
        {
            data = new[]
            {
                new Dummy
                {
                    Id = 1,
                    Name = "Asdf",
                    Number = 123.45f
                },
                new Dummy
                {
                    Id = 2,
                    Name = "Qwer",
                    Number = 67.89f
                },
                new Dummy
                {
                    Id = 3,
                    Name = "Narf",
                    Number = 3.14f
                }
            }
            .AsQueryable();
        }

        [Fact]
        public async Task ToListAsyncShouldFail()
        {
            _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                  data.ToListAsync());
        }

        [Fact]
        public async Task ToListAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.DbRewrite(rewriter);

            var result = await query.ToListAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task SumAsyncShouldFail()
        {
            _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                  data.SumAsync(d => d.Number));
        }

        [Fact]
        public async Task SumAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.DbRewrite(rewriter);

            var result = await query.SumAsync(d => d.Number);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(194.48f, result, 2);
        }

        [Fact]
        public async Task AsyncEnumeratorShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.DbRewrite(rewriter);

            var enumerator = ((IDbAsyncEnumerable)query).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.DbRewrite(rewriter);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(expression, CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, (int)result);
        }
    }
}
