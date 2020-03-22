namespace NeinLinq.Fakes.PredicateTranslator
{
    public class ChildDummy : IDummy
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public ParentDummy? Parent { get; set; }
    }
}
