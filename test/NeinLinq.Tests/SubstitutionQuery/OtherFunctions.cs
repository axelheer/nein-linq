namespace NeinLinq.Tests.SubstitutionQuery
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
