using System;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class ParameterBinderTest
    {
        readonly ParameterExpression parameter = Expression.Parameter(typeof(object));
        readonly Expression replacement = Expression.Constant(null, typeof(object));

        [Fact]
        public void ConstructorShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new ParameterBinder(parameter, null));
            Assert.Throws<ArgumentNullException>(() => new ParameterBinder(null, replacement));
        }
    }
}
