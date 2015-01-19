using System;
using System.Collections.Generic;

namespace NeinLinq.Tests.Selector
{
    public class DummyOne : Dummy
    {
        public IEnumerable<DummyTwo> Twos { get; set; }
    }
}
