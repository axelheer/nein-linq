using System;

namespace NeinLinq.Tests
{
    public static class SubstitutionFunctionsA
    {
        public static bool IsSomehowCalled { get; set; }

        public static bool IsSomehow(string value)
        {
            IsSomehowCalled = true;
            return true;
        }
    }
}
