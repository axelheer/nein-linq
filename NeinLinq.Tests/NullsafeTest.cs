using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class NullsafeTest
    {
        private readonly IQueryable<NullsafeDummyA> data;

        public NullsafeTest()
        {
            data = new[] {
                new NullsafeDummyA
                {
                    SomeInteger = 7,
                    SomeDate = new DateTime(1977, 05, 25),
                    SomeOther = new NullsafeDummyA { SomeInteger = 42 }
                },
                new NullsafeDummyA
                {
                    SomeInteger = 1138,
                    SomeDate = new DateTime(1980, 05, 21),
                    SomeOthers = new[]
                    {
                        new NullsafeDummyA { SomeDate = new DateTime(2000, 3, 1) },
                        new NullsafeDummyA { SomeDate = new DateTime(2000, 6, 1) }
                    }
                },
                new NullsafeDummyA
                {
                    SomeInteger = 123456,
                    SomeDate = new DateTime(1983, 05, 25),
                    MoreOthers = new[]
                    {
                        new NullsafeDummyA { SomeOther = new NullsafeDummyA { SomeDate = new DateTime(2000, 1, 5) } },
                        new NullsafeDummyA { SomeOther = new NullsafeDummyA { SomeDate = new DateTime(2000, 1, 8) } }
                    }
                },
                new NullsafeDummyA
                {
                    SomeInteger = 654321,
                    SomeDate = new DateTime(2015, 12, 18),
                    EvenLotMoreOthers = new HashSet<NullsafeDummyA>()
                    {
                        new NullsafeDummyA { SomeOther = new NullsafeDummyA { SomeDate = new DateTime(2000, 1, 4) } },
                        new NullsafeDummyA { SomeOther = new NullsafeDummyA { SomeDate = new DateTime(2000, 1, 7) } }
                    }
                },
                null
            }
            .AsQueryable();
        }

        [Fact]
        public void OrdinaryQueryShouldFail()
        {
            Assert.Throws<NullReferenceException>(() =>
                query(data).ToList());
        }

        [Fact]
        public void NullsafeQueryShouldSucceed()
        {
            var result = query(data.ToNullsafe()).ToList();

            Assert.Equal(5, result.Count);

            assertDummy(result[0]);
            assertDummy(result[1], year: 1977, integer: 42);
            assertDummy(result[2], year: 1980, other: new[] { 3, 6 });
            assertDummy(result[3], year: 1983, more: new[] { 5, 8 });
            assertDummy(result[4], year: 2015, lot: new[] { 4, 7 });
        }

        private static IQueryable<NullsafeDummyB> query(IQueryable<NullsafeDummyA> data)
        {
            return from a in data
                   orderby a.SomeInteger
                   select new NullsafeDummyB
                   {
                       Year = a.SomeDate.Year,
                       Integer = a.SomeOther.SomeInteger,
                       Other = from b in a.SomeOthers
                               select b.SomeDate.Month,
                       More = from c in a.MoreOthers
                              select c.SomeOther.SomeDate.Day,
                       Lot = from d in a.EvenLotMoreOthers
                             select d.SomeOther.SomeDate.Day
                   };
        }

        private static void assertDummy(NullsafeDummyB dummy, int year = 0, int integer = 0, int[] other = null, int[] more = null, int[] lot = null)
        {
            Assert.Equal(year, dummy.Year);
            Assert.Equal(integer, dummy.Integer);
            Assert.Equal(other ?? new int[0], dummy.Other);
            Assert.Equal(more ?? new int[0], dummy.More);
            Assert.Equal(lot ?? new int[0], dummy.Lot);
        }
    }
}