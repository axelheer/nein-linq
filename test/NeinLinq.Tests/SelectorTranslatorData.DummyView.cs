namespace NeinLinq.Tests.SelectorTranslatorData
{
    public class DummyView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DummyView()
        {
        }

        public DummyView(int id)
        {
            Id = id;
        }
    }
}
