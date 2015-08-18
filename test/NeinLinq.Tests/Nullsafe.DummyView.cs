using System.Collections.Generic;

namespace NeinLinq.Tests.Nullsafe
{
    public class DummyView
    {
        public int Year { get; set; }

        public int Numeric { get; set; }

        public IEnumerable<int> Other { get; set; }

        public IEnumerable<int> More { get; set; }

        public IEnumerable<int> Lot { get; set; }
    }
}
