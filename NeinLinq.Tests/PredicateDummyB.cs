using System;
using System.Collections.Generic;

namespace NeinLinq.Tests
{
    public class PredicateDummyB : PredicateDummyA
    {
        public IEnumerable<PredicateDummyC> C { get; set; }
    }
}
