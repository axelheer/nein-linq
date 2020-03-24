using System;
using System.Linq.Expressions;
using Xunit;

namespace NeinLinq.Tests
{
    public class CachedExpressionTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(()
                => new CachedExpression<Func<int, int>>(null!));
        }

        [Fact]
        public void ShouldCompileExpression()
        {
            Expression<Func<int, int>> expression = i => i + i;

            var subject = CachedExpression.From(expression);

            Assert.Equal(2, subject.Compiled(1));
        }

        [Fact]
        public void ShouldCastToExpression()
        {
            Expression<Func<int, int>> expected = i => i + i;

            var subject = CachedExpression.From(expected);

            var actual = (Expression<Func<int, int>>)subject;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCastToLambdaExpression()
        {
            Expression<Func<int, int>> expected = i => i + i;

            var subject = CachedExpression.From(expected);

            var actual = (LambdaExpression)subject;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldCastNullToExpression()
        {
            var subject = default(CachedExpression<Func<int, int>>);

            var actual = (Expression<Func<int, int>>)subject!;

            Assert.Null(actual);
        }

        [Fact]
        public void ShouldCastNullToLambdaExpression()
        {
            var subject = default(CachedExpression<Func<int, int>>);

            var actual = (LambdaExpression)subject!;

            Assert.Null(actual);
        }
    }
}
