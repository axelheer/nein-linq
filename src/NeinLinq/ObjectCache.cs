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
            cacheLock.EnterUpgradeableReadLock();

            try
            {
                var value = default(TValue);
                if (cache.TryGetValue(key, out value))
                    return value;

                cacheLock.EnterWriteLock();

                try
                {
                    value = valueFactory(key);
                    cache.Add(key, value);

                    return value;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            cacheLock.Dispose();
        }
    }
}
