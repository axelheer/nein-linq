using System;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class AttributeTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default(string)));
            Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default(Type)));
            Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default(Type), "Narf"));
            Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(typeof(object), default(string)));
        }
    }
}
