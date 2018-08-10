using System.Data.Entity.Infrastructure;
using System.Linq;

namespace NeinLinq.EntityFramework
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteDbQueryable<T> : RewriteQueryable<T>, IDbAsyncEnumerable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="provider">The provider to rewrite the query.</param>
        /// <param name="queryable">The actual query.</param>
        public RewriteDbQueryable(RewriteDbQueryProvider provider, IQueryable queryable)
            : base(provider, queryable)
        {
        }

        /// <inheritdoc />
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            // rewrite on enumeration
            var enumerable = RewriteQuery();
            if (enumerable is IDbAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteDbQueryEnumerator<T>(enumerable.GetEnumerator());
        }

        /// <inheritdoc />
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();
    }
}
