using System;
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

            Assert.Equal(4, result.Count);

            Assert.Equal(0, result[0].Year);
            Assert.Equal(0, result[0].Integer);
            Assert.Equal(new int[0], result[0].Others);
            Assert.Equal(new int[0], result[0].More);

            Assert.Equal(1977, result[1].Year);
            Assert.Equal(42, result[1].Integer);
            Assert.Equal(new int[0], result[1].Others);
            Assert.Equal(new int[0], result[1].More);

            Assert.Equal(1980, result[2].Year);
            Assert.Equal(0, result[2].Integer);
            Assert.Equal(new[] { 3, 6 }, result[2].Others);
            Assert.Equal(new int[0], result[2].More);

            Assert.Equal(1983, result[3].Year);
            Assert.Equal(0, result[3].Integer);
            Assert.Equal(new int[0], result[3].Others);
            Assert.Equal(new[] { 5, 8 }, result[3].More);
        }

        private static IQueryable<NullsafeDummyB> query(IQueryable<NullsafeDummyA> data)
        {
            return from a in data
                   orderby a.SomeInteger
                   select new NullsafeDummyB
                   {
                       Year = a.SomeDate.Year,
                       Integer = a.SomeOther.SomeInteger,
                       Others = from b in a.SomeOthers
                                select b.SomeDate.Month,
                       More = from c in a.MoreOthers
                              select c.SomeOther.SomeDate.Day
                   };
        }
    }
}
