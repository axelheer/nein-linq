using NeinLinq.Fakes.DynamicQuery;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

using static NeinLinq.DynamicQuery;

namespace NeinLinq.Tests.DynamicQuery
{
    public class CreatePredicateTest
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>(null, DynamicCompare.Equal, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreatePredicate<Dummy>("Number", (DynamicCompare)(object)-1, null));
            Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>(null, "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>("Name", null, "b"));
        }

        [Theory]
        [InlineData(DynamicCompare.Equal, new[] { 5 })]
        [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
        [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
        public void ShouldCreateComparison(DynamicCompare comparison, int[] result)
        {
            var culture = new CultureInfo("de-AT");

            var empty = CreatePredicate<Dummy>("Number", comparison, null);
            var compare = CreatePredicate<Dummy>("Number", comparison, "222,222", culture);

            var emptyResult = data.Where(empty).Select(d => d.Id).ToArray();
            var compareResult = data.Where(compare).Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Fact]
        public void ShouldCreateCustomComparison()
        {
            var contains = CreatePredicate<Dummy>("Name", "Contains", "b");

            var containsResult = data.Where(contains).Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void ShouldSupportGuidsToo()
        {
            var expected = data.First().Reference;

            var predicate = CreatePredicate<Dummy>("Reference", DynamicCompare.Equal, expected.ToString());

            var actual = data.Where(predicate).Select(d => d.Reference).ToArray();

            Assert.Equal(new[] { expected }, actual);
        }
    }
}
