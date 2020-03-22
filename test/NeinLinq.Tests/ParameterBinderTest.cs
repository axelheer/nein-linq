using System;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class ParameterBinderTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ParameterBinder(null!, Expression.Constant(true)));
            _ = Assert.Throws<ArgumentNullException>(() => new ParameterBinder(Expression.Parameter(typeof(bool)), null!));
        }

        [Fact]
        public void ShouldVisitParameter()
        {
            var parameter = Expression.Parameter(typeof(bool));
            var replacement = Expression.Constant(true);

            var result = new ParameterBinder(parameter, replacement).Visit(parameter);

            Assert.Equal(replacement, result);
        }

        [Fact]
        public void ShouldNotVisitParameter()
        {
            var parameter = Expression.Parameter(typeof(bool));
            var replacement = Expression.Constant(true);

            var result = new ParameterBinder(Expression.Parameter(typeof(bool)), replacement).Visit(parameter);

            Assert.Equal(parameter, result);
        }

        [Fact]
        public void ShouldVisitInvocation()
        {
            var parameter = Expression.Parameter(typeof(Func<bool, bool>));
            var replacement = Expression.Lambda<Func<bool, bool>>(Expression.Constant(true), Expression.Parameter(typeof(bool)));

            var result = new ParameterBinder(parameter, replacement).Visit(Expression.Invoke(parameter, Expression.Parameter(typeof(bool))));

            Assert.Equal(ExpressionType.Constant, result.NodeType);
        }

        [Fact]
        public void ShouldNotVisitInvocation()
        {
            var parameter = Expression.Parameter(typeof(Func<bool, bool>));
            var replacement = Expression.Lambda<Func<bool, bool>>(Expression.Constant(true), Expression.Parameter(typeof(bool)));

            var result = new ParameterBinder(Expression.Parameter(typeof(bool)), replacement).Visit(Expression.Invoke(parameter, Expression.Parameter(typeof(bool))));

            Assert.NotEqual(ExpressionType.Constant, result.NodeType);
        }

        [Fact]
        public void ShouldNotVisitInvocationLambda()
        {
            var parameter = Expression.Parameter(typeof(Func<bool, bool>));
            var replacement = Expression.Parameter(typeof(Func<bool, bool>));

            var result = new ParameterBinder(parameter, replacement).Visit(Expression.Invoke(parameter, Expression.Parameter(typeof(bool))));

            Assert.NotEqual(ExpressionType.Constant, result.NodeType);
        }
    }
}
