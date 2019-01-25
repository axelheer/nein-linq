using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Query provider with rewrite capabilities.
    /// </summary>
    public interface IRewriteQueryProvider : IQueryProvider
    {
        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        IQueryable<TElement> RewriteQuery<TElement>(Expression expression);

        /// <summary>
        /// Rewrites the entire query expression.
        /// </summary>
        /// <param name="expression">The query expression.</param>
        /// <returns>A rewritten query.</returns>
        IQueryable RewriteQuery(Expression expression);
    }
}
