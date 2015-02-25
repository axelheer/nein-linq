using System;

namespace NeinLinq.Tests.Selector
{
    public class ChildDummy : IDummy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ParentDummy Parent { get; set; }
    }
}
