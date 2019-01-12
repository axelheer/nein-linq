using System;
using System.Linq.Expressions;
using NeinLinq.Fakes.DynamicQuery;
using Xunit;
using static NeinLinq.DynamicExpression;

namespace NeinLinq.Tests.DynamicQuery
{
    public class CreateComparisonTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => CreateComparison(null, "Number", DynamicCompare.Equal, null));
            Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), null, DynamicCompare.Equal, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), "Number", (DynamicCompare)(object)-1, null));
            Assert.Throws<ArgumentNullException>(() => CreateComparison(null, "Name", "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), null, "Contains", "b"));
            Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), "Name", null, "b"));
        }
    }
}
