using System;
using NeinLinq.Fakes.PredicateTranslator;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class ConstructorTest
    {
        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(()
                => new PredicateTranslation<Dummy>(null!));
        }
    }
}
