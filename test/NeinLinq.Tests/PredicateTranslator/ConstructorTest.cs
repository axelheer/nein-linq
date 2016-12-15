using NeinLinq.Fakes.PredicateTranslator;
using System;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ConstructorTest
    {
        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new PredicateTranslation<Dummy>(null));
        }
    }
}
