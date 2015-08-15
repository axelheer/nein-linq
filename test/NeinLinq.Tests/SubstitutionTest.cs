using NeinLinq.Tests.Substitution;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class SubstitutionTest
    {
        private readonly IQueryable<Dummy> data;

        public SubstitutionTest()
        {
            data = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" }
            }
            .AsQueryable();

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
