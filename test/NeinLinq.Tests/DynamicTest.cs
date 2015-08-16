using NeinLinq.Tests.Dynamic;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class DynamicTest
    {
        readonly IQueryable<Dummy> data;

        public DynamicTest()
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
        public void CreatePredicateShouldCreateComparison()
        {
            var value = (222.222m).ToString(CultureInfo.CurrentCulture);
            var empty = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.Equal, null);
            var equal = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.Equal, value);
            var notEqual = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.NotEqual, value);
            var greaterThan = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.GreaterThan, value);
            var greaterThanOrEqual = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.GreaterThanOrEqual, value);
            var lessThan = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.LessThan, value);
            var lessThanOrEqual = DynamicQuery.CreatePredicate<Dummy>("Number", DynamicCompare.LessThanOrEqual, value);

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
            var contains = DynamicQuery.CreatePredicate<Dummy>("Name", "Contains", "b");

            var containsResult = data.Where(contains).Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void WhereShouldFilterByComparison()
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
        public void WhereShouldFilterByCustomComparison()
        {
            var contains = data.Where("Name", "Contains", "b");

            var containsResult = contains.Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void OrderByShouldSortBySelector()
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
