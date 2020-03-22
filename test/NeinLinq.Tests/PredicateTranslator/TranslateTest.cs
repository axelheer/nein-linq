using System;
using System.Linq;
using System.Linq.Expressions;
using NeinLinq.Fakes.PredicateTranslator;
using Xunit;

namespace NeinLinq.Tests.PredicateTranslator
{
    public class TranslateTest
    {
        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            Expression<Func<Dummy, bool>>? p = null;

            _ = Assert.Throws<ArgumentNullException>(() => p!.Translate());
        }
    }
}
