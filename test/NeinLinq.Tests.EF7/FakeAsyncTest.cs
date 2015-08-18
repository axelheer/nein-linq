using Microsoft.Data.Entity;
using NeinLinq.Tests.FakeAsync;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class FakeAsyncTest
    {
        readonly IQueryable<Dummy> data;

        public FakeAsyncTest()
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
        public async void ToListAsyncShouldFail()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                data.ToListAsync());
        }

        [Fact]
        public async void ToListAsyncShouldSucceed()
        {
            var query = data.Rewrite(new Rewriter());

            var result = await query.ToListAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async void SumAsyncShouldFail()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                data.SumAsync(d => d.Number));
        }

        [Fact]
        public async void SumAsyncShouldSucceed()
        {
            var query = data.Rewrite(new Rewriter());

            var result = await query.SumAsync(d => d.Number);

            Assert.Equal(194.48m, result, 2);
        }
    }
}
