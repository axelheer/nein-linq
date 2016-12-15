using System;
using System.Collections.Generic;

namespace NeinLinq.Fakes.NullsafeQuery
{
    public class Dummy
    {
        public int SomeNumeric { get; set; }

        public string SomeText { get; set; }

        public DateTime OneDay { get; set; }

        public Dummy SomeOther { get; set; }

        public IEnumerable<Dummy> SomeOthers { get; set; }

        public ICollection<Dummy> MoreOthers { get; set; }

        public ISet<Dummy> EvenLotMoreOthers { get; set; }
    }
}
