using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NeinLinq.Tests.Nullsafe
{
    public class Dummy
    {
        public int SomeNumeric { get; set; }

        public DateTime OneDay { get; set; }

        public Dummy SomeOther { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IEnumerable<Dummy> SomeOthers { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<Dummy> MoreOthers { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ISet<Dummy> EvenLotMoreOthers { get; set; }
    }
}
