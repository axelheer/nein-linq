using System.Collections.Generic;

namespace NeinLinq.Tests.Selector
{
    public class ParentDummyView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ChildDummyView FirstChild { get; set; }

        public IEnumerable<ChildDummyView> Children { get; set; }
    }
}
