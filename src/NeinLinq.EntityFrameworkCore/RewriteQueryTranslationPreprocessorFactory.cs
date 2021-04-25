using Microsoft.EntityFrameworkCore.Query;

#pragma warning disable CA1812

namespace NeinLinq
{
    internal class RewriteQueryTranslationPreprocessorFactory<TInnerFactory> : IQueryTranslationPreprocessorFactory
        where TInnerFactory : class, IQueryTranslationPreprocessorFactory
    {
        private readonly RewriteQueryTranslationPreprocessorOptions options;
        private readonly QueryTranslationPreprocessorDependencies dependencies;
        private readonly TInnerFactory innerFactory;

        public RewriteQueryTranslationPreprocessorFactory(RewriteQueryTranslationPreprocessorOptions options,
                                                          QueryTranslationPreprocessorDependencies dependencies,
                                                          TInnerFactory innerFactory)
        {
            this.options = options;
            this.dependencies = dependencies;
            this.innerFactory = innerFactory;
        }

        public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
            => new RewriteQueryTranslationPreprocessor(options, dependencies, queryCompilationContext, innerFactory.Create(queryCompilationContext));
    }
}
