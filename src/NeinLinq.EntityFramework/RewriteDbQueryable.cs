using System.Data.Entity.Infrastructure;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteDbQueryable<T> : RewriteQueryable<T>, IDbAsyncEnumerable<T>
    {
        private readonly RewriteDbQueryProvider provider;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="queryable">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteDbQueryable(IQueryable queryable, RewriteDbQueryProvider provider)
            : base(queryable, provider)
        {
            this.provider = provider;
        }

        /// <inheritdoc />
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            // rewrite on enumeration
            var enumerable = provider.RewriteQuery<T>(Expression);
            if (enumerable is IDbAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteDbQueryEnumerator<T>(enumerable.GetEnumerator());
        }

        /// <inheritdoc />
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();
    }
}
