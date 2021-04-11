using System;

namespace NeinLinq
{
    internal class InjectableQueryTranslationPreprocessorOptions
    {
        public Type[] Greenlist { get; }

        public InjectableQueryTranslationPreprocessorOptions(Type[] greenlist)
        {
            Greenlist = greenlist ?? throw new ArgumentNullException(nameof(greenlist));
        }
    }
}
