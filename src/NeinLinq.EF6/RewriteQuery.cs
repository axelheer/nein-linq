using System;
using System.Data.Entity.Infrastructure;

namespace NeinLinq
{
    public abstract partial class RewriteQuery : IDbAsyncEnumerable
    {
        /// <inheritdoc />
        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return (RewriteQueryEnumerator)Activator.CreateInstance(
                    typeof(RewriteQueryEnumerator<>).MakeGenericType(elementType),
                    enumerable.Value.GetEnumerator());
        }
    }

    public partial class RewriteQuery<T> : IDbAsyncEnumerable<T>
    {
        /// <inheritdoc />
        public new IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            var asyncEnumerable = enumerable.Value as IDbAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return new RewriteQueryEnumerator<T>(enumerable.Value.GetEnumerator());
        }
    }
}
