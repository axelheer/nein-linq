using System.Collections.Generic;

namespace NeinLinq.Fakes.DynamicQuery
{
    public static class DummyStore
    {
        public static IList<Dummy> Data { get; } = new[]
        {
            new Dummy { Id = 1, Name = "aaaa", Number = 11.11m },
            new Dummy { Id = 2, Name = "bbbb", Number = 22.22m },
            new Dummy { Id = 3, Name = "cccc", Number = 33.33m },
            new Dummy { Id = 4, Name = "aaa", Number = 111.111m },
            new Dummy { Id = 5, Name = "bbb", Number = 222.222m },
            new Dummy { Id = 6, Name = "ccc", Number = 333.333m },
            new Dummy { Id = 7, Name = "aa", Number = 1111.1111m },
            new Dummy { Id = 8, Name = "bb", Number = 2222.2222m },
            new Dummy { Id = 9, Name = "cc", Number = 3333.3333m }
        };
    }
}
