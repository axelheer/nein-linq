using System;
using System.Diagnostics.CodeAnalysis;

namespace NeinLinq
{
    [ExcludeFromCodeCoverage]
    internal class InjectableQueryTranslationPreprocessorOptions
    {
        public Type[] Greenlist { get; }

        public InjectableQueryTranslationPreprocessorOptions(Type[] greenlist)
        {
            Greenlist = greenlist ?? throw new ArgumentNullException(nameof(greenlist));
        }
    }
}
