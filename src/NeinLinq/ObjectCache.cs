using System;
using System.Collections.Generic;
using System.Threading;

namespace NeinLinq
{
    sealed class ObjectCache<TKey, TValue> : IDisposable
    {
        readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

        readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var value = default(TValue);

            cacheLock.EnterReadLock();

            try
            {
                if (cache.TryGetValue(key, out value))
                    return value;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            cacheLock.EnterWriteLock();

            try
            {
                if (cache.TryGetValue(key, out value))
                    return value;

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
        {
            cacheLock.Dispose();
        }
    }
}
