using System;
using System.Collections.Generic;

namespace NeinLinq.Fakes.DynamicQuery
{
    public static class DummyStore
    {
        public static IEnumerable<Dummy> Data { get; }
            = new[]
            {
                new Dummy { Id = 1, Name = "aaaa", Number = 11.11m, Reference = Guid.NewGuid(), Enum = DummyEnum.Undefined, NullableEnum = DummyEnum.Two },
                new Dummy { Id = 2, Name = "bbbb", Number = 22.22m, Reference = Guid.NewGuid(), Enum = DummyEnum.Undefined, NullableEnum = DummyEnum.Two},
                new Dummy { Id = 3, Name = "cccc", Number = 33.33m, Reference = Guid.NewGuid(), Enum = DummyEnum.Undefined, NullableEnum = null},
                new Dummy { Id = 4, Name = "aaa", Number = 111.111m, Reference = Guid.NewGuid(), Enum = DummyEnum.One, NullableEnum = DummyEnum.Undefined },
                new Dummy { Id = 5, Name = "bbb", Number = 222.222m, Reference = Guid.NewGuid(), Enum = DummyEnum.One, NullableEnum = DummyEnum.Undefined },
                new Dummy { Id = 6, Name = "ccc", Number = 333.333m, Reference = Guid.NewGuid(), Enum = DummyEnum.One, NullableEnum = null },
                new Dummy { Id = 7, Name = "aa", Number = 1111.1111m, Reference = Guid.NewGuid(), Enum = DummyEnum.Two, NullableEnum = DummyEnum.One},
                new Dummy { Id = 8, Name = "bb", Number = 2222.2222m, Reference = Guid.NewGuid(), Enum = DummyEnum.Two, NullableEnum = DummyEnum.One },
                new Dummy { Id = 9, Name = "cc", Number = 3333.3333m, Reference = Guid.NewGuid(), Enum = DummyEnum.Two, NullableEnum = null }
            };
    }
}
