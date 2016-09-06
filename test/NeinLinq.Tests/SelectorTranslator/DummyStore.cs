using System.Collections.Generic;
using System.Linq;

namespace NeinLinq.Tests.SelectorTranslator
{
    public static class DummyStore
    {
        public static IReadOnlyList<IDummy> Data { get; } = InitData();

        static IReadOnlyList<IDummy> InitData()
        {
            var d = new[]
            {
                new Dummy { Id = 1, Name = "Asdf" },
                new Dummy { Id = 2, Name = "Narf" },
                new Dummy { Id = 3, Name = "Qwer" }
            };

            var s = new[]
            {
                new SuperDummy { Id = 4, Name = "Asdf", Description = "Asdf" },
                new SuperDummy { Id = 5, Name = "Narf", Description = "Narf" },
                new SuperDummy { Id = 6, Name = "Qwer", Description = "Qwer" }
            };

            var p = new[]
            {
                new ParentDummy { Id = 7, Name = "Asdf" },
                new ParentDummy { Id = 8, Name = "Narf" },
                new ParentDummy { Id = 9, Name = "Qwer" }
            };

            var c = new[]
            {
                new ChildDummy { Id = 10, Name = "Asdf", Parent = p[1] },
                new ChildDummy { Id = 11, Name = "Narf", Parent = p[2] },
                new ChildDummy { Id = 12, Name = "Qwer", Parent = p[0] }
            };

            p[0].Children = new[] { c[0], c[1] };
            p[1].Children = new[] { c[1], c[2] };
            p[2].Children = new[] { c[2], c[0] };

            return d.Concat<IDummy>(s).Concat(p).Concat(c).ToList();
        }
    }
}
