using System;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class DynamicExpressionTest
    {
        [Fact]
        public void OperatorCreateComparison_NullArgument_Throws()
        {
            var target = Expression.Parameter(typeof(Model));
            var selector = "selector";
            var comparer = DynamicCompare.Equal;

            var targetError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateComparison(null!, selector, comparer, null));
            var selectorError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateComparison(target, null!, comparer, null));
            var comparerError = Assert.Throws<ArgumentOutOfRangeException>(()
                => DynamicExpression.CreateComparison(target, selector, (DynamicCompare)(object)-1, null));

            Assert.Equal("target", targetError.ParamName);
            Assert.Equal("selector", selectorError.ParamName);
            Assert.Equal("comparer", comparerError.ParamName);
        }

        [Fact]
        public void MethodCreateComparison_NullArgument_Throws()
        {
            var target = Expression.Parameter(typeof(Model));
            var selector = "selector";
            var comparer = "comparer";

            var targetError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateComparison(null!, selector, comparer, null));
            var selectorError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateComparison(target, null!, comparer, null));
            var comparerError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateComparison(target, selector, null!, null));

            Assert.Equal("target", targetError.ParamName);
            Assert.Equal("selector", selectorError.ParamName);
            Assert.Equal("comparer", comparerError.ParamName);
        }

        [Fact]
        public void CreateMemberAccess_NullArgument_Throws()
        {
            var target = Expression.Parameter(typeof(Model));
            var selector = "selector";

            var targetError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateMemberAccess(null!, selector));
            var selectorError = Assert.Throws<ArgumentNullException>(()
                => DynamicExpression.CreateMemberAccess(target, null!));

            Assert.Equal("target", targetError.ParamName);
            Assert.Equal("selector", selectorError.ParamName);
        }

#pragma warning disable CA1812

        private class Model
        {
        }
    }
}
