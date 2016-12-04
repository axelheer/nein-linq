
using System.Linq;

#if EF

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#elif EFCore

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#endif

namespace NeinLinq.Tests.FakeAsyncQuery
{
    public class Test
    {
        readonly IQueryable<Dummy> data;

        public Test()
        {
            data = new[]
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
            }
            .AsQueryable();
        }

#if EF || EFCore

        [Fact]
        public async Task ToListAsyncShouldFail()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                data.ToListAsync());
        }

        [Fact]
        public async Task ToListAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.Rewrite(rewriter);

            var result = await query.ToListAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task SumAsyncShouldFail()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                data.SumAsync(d => d.Number));
        }

        [Fact]
        public async Task SumAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.Rewrite(rewriter);

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
            var query = data.Rewrite(rewriter);

            var enumerator = ((IDbAsyncEnumerable)query).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.Rewrite(rewriter);

            var expression = Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { typeof(Dummy) }, query.Expression);

            var result = await ((IDbAsyncQueryProvider)query.Provider).ExecuteAsync(expression, CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(3, (int)result);
        }

#elif EFCore

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.Rewrite(rewriter);

            var enumerator = ((IAsyncQueryProvider)query.Provider).ExecuteAsync<Dummy>(query.Expression).GetEnumerator();

            var result = await enumerator.MoveNext(CancellationToken.None);

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }

#endif

    }
}
