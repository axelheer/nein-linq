#pragma warning disable CA1801
#pragma warning disable CA1822

namespace NeinLinq.Fakes.SubstitutionQuery
{
    public static class OtherFunctions
    {
        public static bool IsSomehowCalled { get; set; }

        public static bool IsSomehow(string value)
        {
            IsSomehowCalled = true;
            return true;
        }
    }
}
