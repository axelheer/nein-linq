using System.Linq;
using NeinLinq.Fakes.SubstitutionQuery;
using Xunit;

namespace NeinLinq.Tests.SubstitutionQuery
{
    public class QueryTest
    {
        private readonly IQueryable<Dummy> data = DummyStore.Data.AsQueryable();

        public QueryTest()
        {
            Functions.IsSomehowCalled = false;
            OtherFunctions.IsSomehowCalled = false;
        }

        [Fact]
        public void FunctionsShouldBeCalled()
        {
            var query = from d in data
                        where Functions.IsSomehow(d.Name)
                        select d;

            var result = query.ToList();

            Assert.True(Functions.IsSomehowCalled);
            Assert.False(OtherFunctions.IsSomehowCalled);
        }

        [Fact]
        public void OtherFunctionsShouldBeCalled()
        {
            var query = from d in data.ToSubstitution(typeof(Functions), typeof(OtherFunctions))
                        where Functions.IsSomehow(d.Name)
                        select d;

            var result = query.ToList();

            Assert.False(Functions.IsSomehowCalled);
            Assert.True(OtherFunctions.IsSomehowCalled);
        }
    }
}
