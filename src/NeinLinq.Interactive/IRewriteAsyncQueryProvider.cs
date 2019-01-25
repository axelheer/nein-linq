using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Query provider with rewrite capabilities.
    /// </summary>
    public interface IRewriteAsyncQueryProvider : IAsyncQueryProvider
    {
        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        IAsyncQueryable<TElement> RewriteQuery<TElement>(Expression expression);
    }
}
