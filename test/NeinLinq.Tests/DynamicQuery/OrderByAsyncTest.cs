using System;
using System.Linq;
using System.Threading.Tasks;
using NeinLinq.Fakes.DynamicQuery;
using Xunit;

namespace NeinLinq.Tests.DynamicQuery
{
    public class AsyncOrderByTest
    {
        private readonly IAsyncQueryable<Dummy> data = DummyStore.Data.ToAsyncEnumerable().AsAsyncQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IAsyncQueryable<Dummy>).OrderBy("Name.Length"));
            Assert.Throws<ArgumentNullException>(() => data.OrderBy(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedAsyncQueryable<Dummy>).ThenBy("Name"));
            Assert.Throws<ArgumentNullException>(() => data.OrderBy("Name.Length").ThenBy(null));
        }

        [Fact]
        public async Task ShouldSortBySelector()
        {
            var one = data.OrderBy("Name.Length").ThenBy("Name", true);
            var two = data.OrderBy("Name.Length", true).ThenBy("Name");

            var oneResult = await one.Select(d => d.Id).ToArrayAsync();
            var twoResult = await two.Select(d => d.Id).ToArrayAsync();

            Assert.Equal(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, oneResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, twoResult);
        }
    }
}
