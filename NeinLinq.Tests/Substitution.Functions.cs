using System;
using System.Diagnostics.CodeAnalysis;

namespace NeinLinq.Tests.Substitution
{
    public static class Functions
    {
        public static bool IsSomehowCalled { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static bool IsSomehow(string value)
        {
            IsSomehowCalled = true;
            return true;
        }
    }
}
