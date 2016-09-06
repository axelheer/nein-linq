using System.Collections.Generic;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ParentDummy : IDummy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ChildDummy> Children { get; set; }
    }
}
