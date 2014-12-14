using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class SubstitutionTest
    {
        private readonly IQueryable<SubstitutionDummy> data;

        public SubstitutionTest()
        {
            data = new[]
            {
                new SubstitutionDummy { Id = 1, Name = "Asdf" },
                new SubstitutionDummy { Id = 2, Name = "Narf" },
                new SubstitutionDummy { Id = 3, Name = "Qwer" }
            }.AsQueryable();
        }

        [Fact]
        public void FunctionsAShouldBeCalled()
        {
            var query = from d in data
                        where SubstitutionFunctionsA.IsSomehow(d.Name)
                        select d;
            SubstitutionFunctionsA.IsSomehowCalled = false;
            SubstitutionFunctionsB.IsSomehowCalled = false;

            var result = query.ToList();

            Assert.True(SubstitutionFunctionsA.IsSomehowCalled);
            Assert.False(SubstitutionFunctionsB.IsSomehowCalled);
        }

        [Fact]
        public void FunctionsBShouldBeCalled()
        {
            var query = from d in data.ToSubstitution(typeof(SubstitutionFunctionsA), typeof(SubstitutionFunctionsB))
                        where SubstitutionFunctionsA.IsSomehow(d.Name)
                        select d;
            SubstitutionFunctionsA.IsSomehowCalled = false;
            SubstitutionFunctionsB.IsSomehowCalled = false;

            var result = query.ToList();

            Assert.False(SubstitutionFunctionsA.IsSomehowCalled);
            Assert.True(SubstitutionFunctionsB.IsSomehowCalled);
        }
    }
}
