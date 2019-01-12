using System;
using System.Linq.Expressions;
using NeinLinq.Fakes.DynamicQuery;
using Xunit;
using static NeinLinq.DynamicExpression;

namespace NeinLinq.Tests.DynamicQuery
{
    public class CreateMemberAccessTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => CreateMemberAccess(null, "Name"));
            Assert.Throws<ArgumentNullException>(() => CreateMemberAccess(Expression.Parameter(typeof(Dummy)), null));
        }
    }
}
