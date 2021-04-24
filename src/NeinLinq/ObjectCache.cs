using System;
using System.Collections.Generic;
using System.Threading;

namespace NeinLinq
{
    internal sealed class ObjectCache<TKey, TValue> : IDisposable
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> cache
            = new();

        private readonly ReaderWriterLockSlim cacheLock
            = new();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            cacheLock.EnterReadLock();

            try
            {
                if (cache.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            cacheLock.EnterWriteLock();

            try
            {
                if (cache.TryGetValue(key, out var value))
                {
                    return value;
                }

                value = valueFactory(key);
                cache.Add(key, value);

                return value;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public void Dispose()
            => cacheLock.Dispose();
    }
}
