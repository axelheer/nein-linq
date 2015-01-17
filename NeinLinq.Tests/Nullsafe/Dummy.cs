using System;
using System.Collections.Generic;

namespace NeinLinq.Tests.Nullsafe
{
    public class Dummy
    {
        public int SomeInteger { get; set; }

        public DateTime SomeDate { get; set; }

        public Dummy SomeOther { get; set; }

        public IEnumerable<Dummy> SomeOthers { get; set; }

        public ICollection<Dummy> MoreOthers { get; set; }

        public ISet<Dummy> EvenLotMoreOthers { get; set; }
    }
}