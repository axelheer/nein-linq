using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

#pragma warning disable CA1812

namespace NeinLinq
{
    internal class RewriteQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        private readonly RewriteQueryTranslationPreprocessorOptions options;
        private readonly QueryTranslationPreprocessor innerPreprocessor;

        public RewriteQueryTranslationPreprocessor(RewriteQueryTranslationPreprocessorOptions options,
                                                   QueryTranslationPreprocessorDependencies dependencies,
                                                   QueryCompilationContext queryCompilationContext,
                                                   QueryTranslationPreprocessor innerPreprocessor)
            : base(dependencies, queryCompilationContext)
        {
            this.options = options;
            this.innerPreprocessor = innerPreprocessor;
        }

        public override Expression Process(Expression query)
            => innerPreprocessor.Process(options.Rewriters.Aggregate(query, (q, r) => r.Visit(q)));
    }
}
