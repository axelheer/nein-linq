using System;
using System.Collections.Generic;

namespace NeinLinq.Tests.Selector
{
    public class DummyOneView : DummyView
    {
        public IEnumerable<DummyTwoView> Twos { get; set; }
    }
}
