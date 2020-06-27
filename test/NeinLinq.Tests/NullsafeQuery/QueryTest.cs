using System;
using System.Linq;
using NeinLinq.Fakes.NullsafeQuery;
using Xunit;

#pragma warning disable CA1307

namespace NeinLinq.Tests.NullsafeQuery
{
    public class QueryTest
    {
        private readonly IQueryable<Dummy?> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldSelectStructMember()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Year = a.OneDay.Year
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(1, r.Year),
                r => Assert.Equal(1977, r.Year),
                r => Assert.Equal(1980, r.Year),
                r => Assert.Equal(1983, r.Year),
                r => Assert.Equal(2015, r.Year));
        }

        [Fact]
        public void ShouldSelectClassMember()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Numeric = a.SomeOther!.SomeNumeric
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(42, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric));
        }

        [Fact]
        public void ShouldSelectMethodCall()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Question = a.SomeText!.Contains("?")
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.False(r.Question),
                r => Assert.False(r.Question),
                r => Assert.True(r.Question),
                r => Assert.False(r.Question),
                r => Assert.False(r.Question));
        }

        [Fact]
        public void ShouldSelectMethodCallResult()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            FirstWord = a.SomeText!.Split(new[] { ' ' }).FirstOrDefault(),
                            CharacterCount = a.SomeText!.ToCharArray().GetLength(0)
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Null(r.FirstWord),
                r => Assert.Equal("Narf", r.FirstWord),
                r => Assert.Equal("What", r.FirstWord),
                r => Assert.Null(r.FirstWord),
                r => Assert.Equal("", r.FirstWord));

            Assert.Collection(result,
                r => Assert.Equal(0, r.CharacterCount),
                r => Assert.Equal(4, r.CharacterCount),
                r => Assert.Equal(20, r.CharacterCount),
                r => Assert.Equal(0, r.CharacterCount),
                r => Assert.Equal(0, r.CharacterCount));
        }

        [Fact]
        public void ShouldSelectEnumerable()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Other = from b in a.SomeOthers
                                    select b.OneDay.Month
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.Other),
                r => Assert.Empty(r.Other),
                r => Assert.Equal(new[] { 1, 3, 6 }, r.Other),
                r => Assert.Empty(r.Other),
                r => Assert.Empty(r.Other));
        }

        [Fact]
        public void ShouldSelectCollection()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            More = from c in a.MoreOthers
                                   select c.SomeOther!.OneDay.Day
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.More),
                r => Assert.Empty(r.More),
                r => Assert.Empty(r.More),
                r => Assert.Equal(new[] { 1, 1, 5, 8 }, r.More),
                r => Assert.Empty(r.More));
        }

        [Fact]
        public void ShouldSelectSet()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Lot = from d in a.EvenLotMoreOthers
                                  select d.SomeOther!.OneDay.Day
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Empty(r.Lot),
                r => Assert.Equal(new[] { 1, 1, 4, 7 }, r.Lot));
        }

        [Fact]
        public void ShouldSelectNullables()
        {
            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Numeric = a.DaNullable!.Value
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(2017, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric));
        }

        [Fact]
        public void ShouldResolveNestedExtensions()
        {
            var query = from a in data.ToNullsafe()
                        group a by a.MoreOthers!.Count into g
                        where g.Key > 0
                        select g.Sum(b => b.MoreOthers!.Sum(c => c!.SomeOther!.OneDay.Month));

            var result = query.ToList();

            Assert.Equal(new[] { 4 }, result);
        }

        [Fact]
        public void ShouldIgnoreStaticMember()
        {
            var query = from _ in Enumerable.Range(1, 3).AsQueryable().ToNullsafe()
                        select new
                        {
                            Property = Guid.Empty,
                            Method = Guid.NewGuid()
                        };

            var result = query.ToList();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void ShouldBeNice()
        {
            var query = from a in data.ToNullsafe()
                        let x = a.SomeOthers!.FirstOrDefault()
                        let y = a.MoreOthers!.FirstOrDefault()
                        let z = a.EvenLotMoreOthers!.FirstOrDefault()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Year = x.OneDay.Year,
                            Numeric = y.SomeOther!.SomeNumeric,
                            Question = z.SomeText!.Contains("?")
                        };

            var result = query.ToList();
        }

        [Fact]
        public void ShouldHandleMember()
        {
            var danger = default(string);

            var query = from a in data.ToNullsafe()
                        orderby a.SomeNumeric
                        select new DummyView
                        {
                            Numeric = danger!.Length
                        };

            var result = query.ToList();

            Assert.Collection(result,
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric),
                r => Assert.Equal(0, r.Numeric));
        }
    }
}
