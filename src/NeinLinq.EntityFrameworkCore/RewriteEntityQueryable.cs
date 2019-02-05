using System.Collections.Generic;
using System.Linq;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteEntityQueryable<T> : RewriteQueryable<T>, IAsyncEnumerable<T>, IQueryable
    {
        private readonly EntityQueryProviderAdapter providerAdapter;

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteEntityQueryable(IQueryable query, RewriteEntityQueryProvider provider)
            : base(query, provider)
        {
            providerAdapter = new EntityQueryProviderAdapter(provider);
        }

        /// <inheritdoc />
        IQueryProvider IQueryable.Provider => providerAdapter; // feign entity query provider

        /// <inheritdoc />
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            // rewrite on enumeration
            var enumerable = Provider.RewriteQuery<T>(Expression);
            if (enumerable is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetEnumerator();
            return new RewriteEntityQueryEnumerator<T>(enumerable.GetEnumerator());
        }
    }
}
