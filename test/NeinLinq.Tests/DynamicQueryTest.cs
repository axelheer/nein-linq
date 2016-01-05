using NeinLinq.Tests.DynamicQueryData;
using System;
using System.Globalization;
using System.Linq;
using Xunit;
using static NeinLinq.DynamicQuery;

namespace NeinLinq.Tests
{
    public class DynamicQueryTest
    {
        readonly IQueryable<Dummy> data;

        public DynamicQueryTest()
        {
            data = new[]
            {
                new Dummy { Id = 1, Name = "aaaa", Number = 11.11m },
                new Dummy { Id = 2, Name = "bbbb", Number = 22.22m },
                new Dummy { Id = 3, Name = "cccc", Number = 33.33m },
                new Dummy { Id = 4, Name = "aaa", Number = 111.111m },
                new Dummy { Id = 5, Name = "bbb", Number = 222.222m },
                new Dummy { Id = 6, Name = "ccc", Number = 333.333m },
                new Dummy { Id = 7, Name = "aa", Number = 1111.1111m },
                new Dummy { Id = 8, Name = "bb", Number = 2222.2222m },
                new Dummy { Id = 9, Name = "cc", Number = 3333.3333m }
            }
            .AsQueryable();
        }

        [Fact]
        public void CreatePredicateShouldHandleInvalidArguments()
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
        public void CreatePredicateShouldCreateComparison(DynamicCompare comparison, int[] result)
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);

            var empty = CreatePredicate<Dummy>("Number", comparison, null);
            var compare = CreatePredicate<Dummy>("Number", comparison, value);

            var emptyResult = data.Where(empty).Select(d => d.Id).ToArray();
            var compareResult = data.Where(compare).Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Fact]
        public void CreatePredicateShouldCreateCustomComparison()
        {
            var contains = CreatePredicate<Dummy>("Name", "Contains", "b");

            var containsResult = data.Where(contains).Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void WhereUntypedShouldHandleInvalidArguments()
        {
            var weak = (IQueryable)data;

            Assert.Throws<ArgumentNullException>(() => default(IQueryable).Where("Number", DynamicCompare.Equal, null));
            Assert.Throws<ArgumentNullException>(() => weak.Where(null, DynamicCompare.Equal, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => weak.Where("Number", (DynamicCompare)(object)-1, null));
            Assert.Throws<ArgumentNullException>(() => default(IQueryable).Where("Name", "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => weak.Where(null, "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => weak.Where("Name", null, "b"));
        }

        [Fact]
        public void WhereTypedShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).Where("Number", DynamicCompare.Equal, null));
            Assert.Throws<ArgumentNullException>(() => data.Where(null, DynamicCompare.Equal, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => data.Where("Number", (DynamicCompare)(object)-1, null));
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).Where("Name", "Contains", "b"));
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
        public void WhereUntypedShouldFilterByComparison(DynamicCompare comparison, int[] result)
        {
            var weak = (IQueryable)data;

            var value = (222.222m).ToString(CultureInfo.CurrentCulture);

            var empty = weak.Where("Number", comparison, null);
            var compare = weak.Where("Number", comparison, value);

            var emptyResult = empty.Cast<Dummy>().Select(d => d.Id).ToArray();
            var compareResult = compare.Cast<Dummy>().Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Theory]
        [InlineData(DynamicCompare.Equal, new[] { 5 })]
        [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
        [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
        public void WhereTypedShouldFilterByComparison(DynamicCompare comparison, int[] result)
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);

            var empty = data.Where("Number", comparison, null);
            var compare = data.Where("Number", comparison, value);

            var emptyResult = empty.Cast<Dummy>().Select(d => d.Id).ToArray();
            var compareResult = compare.Cast<Dummy>().Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Fact]
        public void WhereUntypedShouldFilterByCustomComparison()
        {
            var weak = (IQueryable)data;

            var contains = weak.Where("Name", "Contains", "b");

            var containsResult = contains.Cast<Dummy>().Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void WhereTypedShouldFilterByCustomComparison()
        {
            var contains = data.Where("Name", "Contains", "b");

            var containsResult = contains.Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void OrderByUntypedShouldHandleInvalidArguments()
        {
            var weak = (IQueryable)data;

            Assert.Throws<ArgumentNullException>(() => default(IQueryable).OrderBy("Name.Length"));
            Assert.Throws<ArgumentNullException>(() => weak.OrderBy(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable).ThenBy("Name"));
            Assert.Throws<ArgumentNullException>(() => weak.OrderBy("Name.Length").ThenBy(null));
        }

        [Fact]
        public void OrderByTypedShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).OrderBy("Name.Length"));
            Assert.Throws<ArgumentNullException>(() => data.OrderBy(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>).ThenBy("Name"));
            Assert.Throws<ArgumentNullException>(() => data.OrderBy("Name.Length").ThenBy(null));
        }

        [Fact]
        public void OrderByUntypedShouldSortBySelector()
        {
            var weak = (IQueryable)data;

            var one = weak.OrderBy("Name.Length").ThenBy("Name", true);
            var two = weak.OrderBy("Name.Length", true).ThenBy("Name");

            var oneResult = one.Cast<Dummy>().Select(d => d.Id).ToArray();
            var twoResult = two.Cast<Dummy>().Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, oneResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, twoResult);
        }

        [Fact]
        public void OrderByTypedShouldSortBySelector()
        {
            var one = data.OrderBy("Name.Length").ThenBy("Name", true);
            var two = data.OrderBy("Name.Length", true).ThenBy("Name");

            var oneResult = one.Select(d => d.Id).ToArray();
            var twoResult = two.Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, oneResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, twoResult);
        }
    }
}
