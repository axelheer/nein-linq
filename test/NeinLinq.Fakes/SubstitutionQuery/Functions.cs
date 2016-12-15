namespace NeinLinq.Fakes.SubstitutionQuery
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
