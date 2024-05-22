namespace NeinLinq;

internal sealed class ObjectCache<TKey, TValue> : IDisposable
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> cache
        = new();

    private readonly ReaderWriterLockSlim cacheLock
        = new();

    public void Add(TKey key, TValue value)
    {
        cacheLock.EnterWriteLock();

        try
        {
            cache.Add(key, value);
        }
        finally
        {
            cacheLock.ExitWriteLock();
        }
    }

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
