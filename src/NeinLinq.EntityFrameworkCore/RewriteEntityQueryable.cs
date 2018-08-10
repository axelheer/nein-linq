using System.Collections.Generic;
using System.Linq;

namespace NeinLinq.EntityFrameworkCore
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteEntityQueryable<T> : RewriteQueryable<T>, IAsyncEnumerable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="provider">The provider to rewrite the query.</param>
        /// <param name="queryable">The actual query.</param>
        public RewriteEntityQueryable(RewriteEntityQueryProvider provider, IQueryable queryable)
            : base(provider, queryable)
        {
        }

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            // rewrite on enumeration
            var enumerable = RewriteQuery();
            if (enumerable is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetEnumerator();
            return new RewriteEntityQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
