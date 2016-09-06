using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests.NullsafeQuery
{
    public class Test
    {
        readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        [Fact]
        public void OrdinaryShouldFail()
        {
            Assert.Throws<NullReferenceException>(() => Query(data).ToList());
        }

        [Fact]
        public void NullsafeShouldSucceed()
        {
            var result = Query(data.ToNullsafe()).ToList();

            Assert.Collection(result,
                r => AssertDummy(r),
                r => AssertDummy(r, 1977, 42, firstWord: "Narf"),
                r => AssertDummy(r, 1980, question: true, firstWord: "What", other: new[] { 3, 6 }),
                r => AssertDummy(r, 1983, more: new[] { 5, 8 }),
                r => AssertDummy(r, 2015, firstWord: "", lot: new[] { 4, 7 }));
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
