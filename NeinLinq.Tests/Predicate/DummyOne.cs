using System;
using System.Collections.Generic;

namespace NeinLinq.Tests.Predicate
{
    public class DummyOne : Dummy
    {
        public IEnumerable<DummyTwo> Twos { get; set; }
    }
}
