using System;
using System.Collections.Generic;

namespace NeinLinq.Tests
{
    public class NullsafeDummyA
    {
        public int SomeInteger { get; set; }

        public DateTime SomeDate { get; set; }

        public NullsafeDummyA SomeOther { get; set; }

        public IEnumerable<NullsafeDummyA> SomeOthers { get; set; }

        public ICollection<NullsafeDummyA> MoreOthers { get; set; }
    }
}
