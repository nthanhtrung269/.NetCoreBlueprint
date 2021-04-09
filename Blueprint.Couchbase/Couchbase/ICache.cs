using System;
using System.Collections.Generic;

namespace Blueprint.Couchbase
{
    public interface ICache
    {
        Dictionary<string, T> GetMulti<T>(IEnumerable<string> keys) where T : class;

        T Get<T>(string key, out bool found, params object[] args) where T : class;

        T Get<T>(string key, Action<T> callIfCached, Func<T> callIfNotCached, TimeSpan slidingExpiration, params object[] args) where T : class;

        T Get<T>(string key, Action<T> callIfCached, Func<T> callIfNotCached, DateTime absoluteExpiration, params object[] args) where T : class;

        T Get<T>(string key, Func<T> callIfNotCached, TimeSpan slidingExpiration, params object[] args) where T : class;

        T Get<T>(string key, Func<T> callIfNotCached, DateTime absoluteExpiration, params object[] args) where T : class;

        void Insert(string key, object data, TimeSpan slidingExpiration, params object[] args);

        void Insert(string key, object data, DateTime absoluteExpiration, params object[] args);

        void Remove(string key, params object[] args);

        IEnumerable<string> GetKeys(int limit, string keySetName, string pattern);

        void FlushCache();

        void FlushCache(string setName, string pattern, string authToken);
    }
}
