using System;
using Xunit;

namespace NeinLinq.Tests
{
    public class SelectorTranslationTest
    {
        [Fact]
        public void Ctor_NullArgument_Throws()
        {
            var error = Assert.Throws<ArgumentNullException>(()
                => new SelectorTranslation<Model, ModelView>(null!));

            Assert.Equal("selector", error.ParamName);
        }

#pragma warning disable CA1812

        private class Model
        {
        }

        private class ModelView
        {
        }
    }
}
