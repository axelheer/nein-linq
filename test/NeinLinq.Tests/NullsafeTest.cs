using NeinLinq.Tests.Nullsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class NullsafeTest
    {
        readonly IQueryable<Dummy> data;

        public NullsafeTest()
        {
            data = new[]
            {
                new Dummy
                {
                    SomeNumeric = 7,
                    SomeText = "Narf",
                    OneDay = new DateTime(1977, 05, 25),
                    SomeOther = new Dummy { SomeNumeric = 42 }
                },
                new Dummy
                {
                    SomeNumeric = 1138,
                    SomeText = "What is thy bidding?",
                    OneDay = new DateTime(1980, 05, 21),
                    SomeOthers = new[]
                    {
                        new Dummy { OneDay = new DateTime(2000, 3, 1) },
                        new Dummy { OneDay = new DateTime(2000, 6, 1) }
                    }
                },
                new Dummy
                {
                    SomeNumeric = 123456,
                    SomeText = null,
                    OneDay = new DateTime(1983, 05, 25),
                    MoreOthers = new[]
                    {
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 5) } },
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 8) } }
                    }
                },
                new Dummy
                {
                    SomeNumeric = 654321,
                    SomeText = "",
                    OneDay = new DateTime(2015, 12, 18),
                    EvenLotMoreOthers = new HashSet<Dummy>
                    {
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 4) } },
                        new Dummy { SomeOther = new Dummy { OneDay = new DateTime(2000, 1, 7) } }
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
                Query(data).ToList());
        }

        [Fact]
        public void NullsafeQueryShouldSucceed()
        {
            var result = Query(data.ToNullsafe()).ToList();

            Assert.Equal(5, result.Count);

            AssertDummy(result[0]);
            AssertDummy(result[1], 1977, 42, firstWord: "Narf");
            AssertDummy(result[2], 1980, question: true, firstWord: "What", other: new[] { 3, 6 });
            AssertDummy(result[3], 1983, more: new[] { 5, 8 });
            AssertDummy(result[4], 2015, firstWord: "", lot: new[] { 4, 7 });
        }

        static IQueryable<DummyView> Query(IQueryable<Dummy> data)
        {
            return from a in data
                   orderby a.SomeNumeric
                   select new DummyView
                   {
                       Year = a.OneDay.Year,
                       Numeric = a.SomeOther.SomeNumeric,
                       Question = a.SomeText.Contains("?"),
                       FirstWord = a.SomeText.Split(' ').FirstOrDefault(),
                       Other = from b in a.SomeOthers
                               select b.OneDay.Month,
                       More = from c in a.MoreOthers
                              select c.SomeOther.OneDay.Day,
                       Lot = from d in a.EvenLotMoreOthers
                             select d.SomeOther.OneDay.Day
                   };
        }

        static void AssertDummy(DummyView dummy,
                                int year = 0,
                                int numeric = 0,
                                bool question = false,
                                string firstWord = null,
                                int[] other = null,
                                int[] more = null,
                                int[] lot = null)
        {
            Assert.Equal(year, dummy.Year);
            Assert.Equal(numeric, dummy.Numeric);
            Assert.Equal(question, dummy.Question);
            Assert.Equal(firstWord, dummy.FirstWord);
            Assert.Equal(other ?? new int[0], dummy.Other);
            Assert.Equal(more ?? new int[0], dummy.More);
            Assert.Equal(lot ?? new int[0], dummy.Lot);
        }
    }
}
