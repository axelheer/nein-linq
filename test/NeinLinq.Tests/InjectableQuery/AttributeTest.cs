using System;
using Xunit;

namespace NeinLinq.Tests.InjectableQuery
{
    public class AttributeTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default(string)!));
            _ = Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default(Type)!));
            _ = Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(default!, "Narf"));
            _ = Assert.Throws<ArgumentNullException>(() => new InjectLambdaAttribute(typeof(object), default!));
        }
    }
}
