using NeinLinq.Fakes.DynamicQuery;
using NeinLinq.Interactive;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NeinLinq.Tests.DynamicQuery
{
    public class AsyncWhereTest
    {
        readonly IAsyncQueryable<Dummy> data = DummyStore.Data.ToAsyncEnumerable().AsAsyncQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IAsyncQueryable<Dummy>).Where("Number", DynamicCompare.Equal, null));
            Assert.Throws<ArgumentNullException>(() => data.Where(null, DynamicCompare.Equal, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => data.Where("Number", (DynamicCompare)(object)-1, null));
            Assert.Throws<ArgumentNullException>(() => default(IAsyncQueryable<Dummy>).Where("Name", "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => data.Where(null, "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => data.Where("Name", null, "b"));
        }

        [Theory]
        [InlineData(DynamicCompare.Equal, new[] { 5 })]
        [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
        [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
        public async Task ShouldFilterByComparison(DynamicCompare comparison, int[] result)
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);

            var empty = data.Where("Number", comparison, null);
            var compare = data.Where("Number", comparison, value);

            var emptyResult = await empty.Select(d => d.Id).ToArray();
            var compareResult = await compare.Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Fact]
        public async Task ShouldFilterByCustomComparison()
        {
            var contains = data.Where("Name", "Contains", "b");

            var containsResult = await contains.Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }
    }
}
