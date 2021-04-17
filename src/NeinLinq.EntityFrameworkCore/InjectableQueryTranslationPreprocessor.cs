using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

#pragma warning disable CA1812

namespace NeinLinq
{
    [ExcludeFromCodeCoverage]
    internal class InjectableQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        private readonly InjectableQueryTranslationPreprocessorOptions options;
        private readonly QueryTranslationPreprocessor innerPreprocessor;

        public InjectableQueryTranslationPreprocessor(InjectableQueryTranslationPreprocessorOptions options,
                                                      QueryTranslationPreprocessorDependencies dependencies,
                                                      QueryCompilationContext queryCompilationContext,
                                                      QueryTranslationPreprocessor innerPreprocessor)
            : base(dependencies, queryCompilationContext)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.innerPreprocessor = innerPreprocessor ?? throw new ArgumentNullException(nameof(innerPreprocessor));
        }

        public override Expression Process(Expression query)
        {
            var rewriter = new InjectableQueryRewriter(options.Greenlist);

            return innerPreprocessor.Process(rewriter.Visit(query));
        }
    }
}
