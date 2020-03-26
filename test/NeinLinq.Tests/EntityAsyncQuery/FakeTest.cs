using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeinLinq.Fakes.EntityAsyncQuery;
using Xunit;

#pragma warning disable EF1001

using IAsyncQueryProvider = Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider;

namespace NeinLinq.Tests.EntityAsyncQuery
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
            var query = data.EntityRewrite(rewriter);

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
            var query = data.EntityRewrite(rewriter);

            var result = await query.SumAsync(d => d.Number);

            Assert.True(rewriter.VisitCalled);
            Assert.Equal(194.48m, result, 2);
        }

        [Fact]
        public async Task ExecuteAsyncShouldSucceed()
        {
            var rewriter = new Rewriter();
            var query = data.EntityRewrite(rewriter);

            var enumerator = ((IAsyncQueryProvider)query.Provider).ExecuteAsync<IAsyncEnumerable<Dummy>>(query.Expression).GetAsyncEnumerator();

            var result = await enumerator.MoveNextAsync();

            Assert.True(rewriter.VisitCalled);
            Assert.True(result);
        }
    }
}
