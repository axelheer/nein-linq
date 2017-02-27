using System.Collections.Generic;

namespace NeinLinq.Fakes.SubstitutionQuery
{
    public static class DummyStore
    {
        public static IEnumerable<Dummy> Data { get; } = new[]
        {
            new Dummy { Id = 1, Name = "Asdf" },
            new Dummy { Id = 2, Name = "Narf" },
            new Dummy { Id = 3, Name = "Qwer" }
        };
    }
}
