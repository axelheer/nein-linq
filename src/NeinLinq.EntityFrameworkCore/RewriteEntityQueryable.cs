using System.Collections.Generic;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteEntityQueryable<T> : RewriteQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly RewriteEntityQueryProvider provider;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteEntityQueryable(IQueryable queryable, RewriteEntityQueryProvider provider)
            : base(queryable, provider)
        {
            this.provider = provider;
        }

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            // rewrite on enumeration
            var enumerable = provider.RewriteQuery<T>(Expression);
            if (enumerable is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetEnumerator();
            return new RewriteEntityQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
