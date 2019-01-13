using System.Data.Entity.Infrastructure;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteDbQueryable<T> : RewriteQueryable<T>, IDbAsyncEnumerable<T>
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteDbQueryable(IQueryable<T> query, RewriteDbQueryProvider provider)
            : base(query, provider)
        {
        }

        /// <inheritdoc />
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            // rewrite on enumeration
            var enumerable = Provider.RewriteQuery<T>(Expression);
            if (enumerable is IDbAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteDbQueryEnumerator<T>(enumerable.GetEnumerator());
        }

        /// <inheritdoc />
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();
    }
}
