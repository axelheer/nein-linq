using System;
using Xunit;

namespace NeinLinq.Tests.SelectorTranslator
{
    public class ConstructorTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new SelectorTranslation<Dummy, DummyView>(null));
        }
    }
}
