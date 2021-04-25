using System.Collections.Generic;
using System.Linq.Expressions;

namespace NeinLinq
{
    internal class RewriteQueryTranslationPreprocessorOptions
    {
        public IList<ExpressionVisitor> Rewriters { get; }

        public RewriteQueryTranslationPreprocessorOptions(params ExpressionVisitor[] rewriters)
        {
            Rewriters = new List<ExpressionVisitor>(rewriters);
        }
    }
}
