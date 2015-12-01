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

        [Fact]
        public void CreatePredicateShouldCreateComparison()
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);
            var empty = CreatePredicate<Dummy>("Number", DynamicCompare.Equal, null);
            var equal = CreatePredicate<Dummy>("Number", DynamicCompare.Equal, value);
            var notEqual = CreatePredicate<Dummy>("Number", DynamicCompare.NotEqual, value);
            var greaterThan = CreatePredicate<Dummy>("Number", DynamicCompare.GreaterThan, value);
            var greaterThanOrEqual = CreatePredicate<Dummy>("Number", DynamicCompare.GreaterThanOrEqual, value);
            var lessThan = CreatePredicate<Dummy>("Number", DynamicCompare.LessThan, value);
            var lessThanOrEqual = CreatePredicate<Dummy>("Number", DynamicCompare.LessThanOrEqual, value);

            var emptyResult = data.Where(empty).Select(d => d.Id).ToArray();
            var equalResult = data.Where(equal).Select(d => d.Id).ToArray();
            var notEqualResult = data.Where(notEqual).Select(d => d.Id).ToArray();
            var greaterThanResult = data.Where(greaterThan).Select(d => d.Id).ToArray();
            var greaterThanOrEqualResult = data.Where(greaterThanOrEqual).Select(d => d.Id).ToArray();
            var lessThanResult = data.Where(lessThan).Select(d => d.Id).ToArray();
            var lessThanOrEqualResult = data.Where(lessThanOrEqual).Select(d => d.Id).ToArray();

            Assert.Equal(0, emptyResult.Length);
            Assert.Equal(new[] { 5 }, equalResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 6, 7, 8, 9 }, notEqualResult);
            Assert.Equal(new[] { 6, 7, 8, 9 }, greaterThanResult);
            Assert.Equal(new[] { 5, 6, 7, 8, 9 }, greaterThanOrEqualResult);
            Assert.Equal(new[] { 1, 2, 3, 4 }, lessThanResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, lessThanOrEqualResult);
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

        [Fact]
        public void WhereUntypedShouldFilterByComparison()
        {
            var weak = (IQueryable)data;

            var value = (222.222m).ToString(CultureInfo.CurrentCulture);
            var empty = weak.Where("Number", DynamicCompare.Equal, null);
            var equal = weak.Where("Number", DynamicCompare.Equal, value);
            var notEqual = weak.Where("Number", DynamicCompare.NotEqual, value);
            var greaterThan = weak.Where("Number", DynamicCompare.GreaterThan, value);
            var greaterThanOrEqual = weak.Where("Number", DynamicCompare.GreaterThanOrEqual, value);
            var lessThan = weak.Where("Number", DynamicCompare.LessThan, value);
            var lessThanOrEqual = weak.Where("Number", DynamicCompare.LessThanOrEqual, value);

            var emptyResult = empty.Cast<Dummy>().Select(d => d.Id).ToArray();
            var equalResult = equal.Cast<Dummy>().Select(d => d.Id).ToArray();
            var notEqualResult = notEqual.Cast<Dummy>().Select(d => d.Id).ToArray();
            var greaterThanResult = greaterThan.Cast<Dummy>().Select(d => d.Id).ToArray();
            var greaterThanOrEqualResult = greaterThanOrEqual.Cast<Dummy>().Select(d => d.Id).ToArray();
            var lessThanResult = lessThan.Cast<Dummy>().Select(d => d.Id).ToArray();
            var lessThanOrEqualResult = lessThanOrEqual.Cast<Dummy>().Select(d => d.Id).ToArray();

            Assert.Equal(0, emptyResult.Length);
            Assert.Equal(new[] { 5 }, equalResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 6, 7, 8, 9 }, notEqualResult);
            Assert.Equal(new[] { 6, 7, 8, 9 }, greaterThanResult);
            Assert.Equal(new[] { 5, 6, 7, 8, 9 }, greaterThanOrEqualResult);
            Assert.Equal(new[] { 1, 2, 3, 4 }, lessThanResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, lessThanOrEqualResult);
        }

        [Fact]
        public void WhereTypedShouldFilterByComparison()
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);
            var empty = data.Where("Number", DynamicCompare.Equal, null);
            var equal = data.Where("Number", DynamicCompare.Equal, value);
            var notEqual = data.Where("Number", DynamicCompare.NotEqual, value);
            var greaterThan = data.Where("Number", DynamicCompare.GreaterThan, value);
            var greaterThanOrEqual = data.Where("Number", DynamicCompare.GreaterThanOrEqual, value);
            var lessThan = data.Where("Number", DynamicCompare.LessThan, value);
            var lessThanOrEqual = data.Where("Number", DynamicCompare.LessThanOrEqual, value);

            var emptyResult = empty.Select(d => d.Id).ToArray();
            var equalResult = equal.Select(d => d.Id).ToArray();
            var notEqualResult = notEqual.Select(d => d.Id).ToArray();
            var greaterThanResult = greaterThan.Select(d => d.Id).ToArray();
            var greaterThanOrEqualResult = greaterThanOrEqual.Select(d => d.Id).ToArray();
            var lessThanResult = lessThan.Select(d => d.Id).ToArray();
            var lessThanOrEqualResult = lessThanOrEqual.Select(d => d.Id).ToArray();

            Assert.Equal(0, emptyResult.Length);
            Assert.Equal(new[] { 5 }, equalResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 6, 7, 8, 9 }, notEqualResult);
            Assert.Equal(new[] { 6, 7, 8, 9 }, greaterThanResult);
            Assert.Equal(new[] { 5, 6, 7, 8, 9 }, greaterThanOrEqualResult);
            Assert.Equal(new[] { 1, 2, 3, 4 }, lessThanResult);
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, lessThanOrEqualResult);
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
