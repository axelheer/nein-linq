using System;
using System.Linq;
using NeinLinq.Fakes.DynamicQuery;
using Xunit;

namespace NeinLinq.Tests.DynamicQuery
{
    public class OrderByUntypedTest
    {
        private readonly IQueryable data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => default(IQueryable)!.OrderBy("Name.Length"));
            _ = Assert.Throws<ArgumentNullException>(() => data.OrderBy(null!));
            _ = Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable)!.ThenBy("Name"));
            _ = Assert.Throws<ArgumentNullException>(() => data.OrderBy("Name.Length").ThenBy(null!));
        }

        [Fact]
        public void ShouldSortBySelector()
        {
            var one = data.OrderBy("Name.Length").ThenBy("Name", true);
            var two = data.OrderBy("Name.Length", true).ThenBy("Name");

            var oneResult = one.Cast<Dummy>().Select(d => d.Id).ToArray();
            var twoResult = two.Cast<Dummy>().Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, oneResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, twoResult);
        }
    }
}
