#pragma warning disable RECS0154

namespace NeinLinq.Tests.SubstitutionQueryData
{
    public static class Functions
    {
        public static bool IsSomehowCalled { get; set; }

        public static bool IsSomehow(string value)
        {
            IsSomehowCalled = true;
            return true;
        }
    }
}

#pragma warning restore RECS0154
