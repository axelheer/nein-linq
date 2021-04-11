using System;
using Microsoft.EntityFrameworkCore.Query;

#pragma warning disable CA1812

namespace NeinLinq
{
    internal class InjectableQueryTranslationPreprocessorFactory<TInnerFactory> : IQueryTranslationPreprocessorFactory
        where TInnerFactory : class, IQueryTranslationPreprocessorFactory
    {
        private readonly InjectableQueryTranslationPreprocessorOptions options;
        private readonly QueryTranslationPreprocessorDependencies dependencies;
        private readonly TInnerFactory innerFactory;

        public InjectableQueryTranslationPreprocessorFactory(InjectableQueryTranslationPreprocessorOptions options,
                                                             QueryTranslationPreprocessorDependencies dependencies,
                                                             TInnerFactory innerFactory)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            this.innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
        }

        public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
            => new InjectableQueryTranslationPreprocessor(options, dependencies, queryCompilationContext, innerFactory.Create(queryCompilationContext));
    }
}
