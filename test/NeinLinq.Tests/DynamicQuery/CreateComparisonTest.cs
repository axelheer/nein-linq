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
            _ = Assert.Throws<ArgumentNullException>(() => CreateComparison(null!, "Number", DynamicCompare.Equal, null!));
            _ = Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), null!, DynamicCompare.Equal, null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), "Number", (DynamicCompare)(object)-1, null!));
            _ = Assert.Throws<ArgumentNullException>(() => CreateComparison(null!, "Name", "Contains", "b"));
            _ = Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), null!, "Contains", "b"));
            _ = Assert.Throws<ArgumentNullException>(() => CreateComparison(Expression.Parameter(typeof(Dummy)), "Name", null!, "b"));
        }
    }
}
