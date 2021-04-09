using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.IO;
using Couchbase.N1QL;
using Couchbase.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Encoding = System.Text.Encoding;

namespace Blueprint.Couchbase
{
    public class CouchbaseCache : ICache, IDisposable
    {
        protected static string BucketName { get; set; }
        protected readonly IBucket _instance;
        protected readonly Cluster _cluster;
        private const string OBJECT_KEY_FORMAT = "{0}:PK:{1}";
        private const string QUERY_KEY_FORMAT = "{0}:QRY:{1}:{2}";
        private const string RELATION_KEY_FORMAT = "{0}:PK:{1}:REL:{2}";
        private CouchbaseConfig CouchbaseConfig { get; set; }
        private bool _disposed = false;

        public CouchbaseCache(List<string> uris, string userName, string password, string bucket)
        {
            CouchbaseConfig = new CouchbaseConfig()
            {
                Uris = uris,
                UserName = userName,
                Password = password,
                Bucket = bucket
            };

            string cbUserName = userName;
            string cbPassword = password;
            string couchbaseBucket = bucket;

            var cbConfig = new ClientConfiguration();
            foreach (var uriString in uris)
            {
                cbConfig.Servers.Add(new Uri(uriString));
            }

            if (_cluster == null)
            {
                _cluster = new Cluster(cbConfig);
            }

            _cluster.Authenticate(cbUserName, cbPassword);
            _instance = _cluster.OpenBucket(couchbaseBucket);
        }

        public static string GetObjectKey(string objName, string objId)
        {
            return string.Format(OBJECT_KEY_FORMAT, objName.ToLower(), objId.ToLower());
        }

        public static string GetQueryKey(string setName, string queryName, params string[] parameters)
        {
            return string.Format(QUERY_KEY_FORMAT, setName.ToLower(), queryName.ToLower(), string.Join(":", parameters).ToLower());
        }

        public static string GetRelationKey(string objName, string objId, string relationName)
        {
            return string.Format(RELATION_KEY_FORMAT, objName.ToLower(), objId.ToLower(), relationName);
        }

        public Dictionary<string, T> GetMulti<T>(IEnumerable<string> keys) where T : class
        {
            return _instance.GetMultiJson<T>(keys.ToList());
        }

        public T Get<T>(string key, out bool found, params object[] args) where T : class
        {
            found = false;
            if (_instance != null)
            {
                key = CreateKey(key, args);
                T o = _instance.GetJson<T>(key);
                found = o != null;
                return o ?? default;
            }
            return default;
        }

        public T Get<T>(string key, Action<T> callIfCached, Func<T> callIfNotCached, TimeSpan slidingExpiration, params object[] args) where T : class
        {
            bool found;
            var item = Get<T>(key, out found, args);
            if (found)
            {
                item = callIfNotCached();
                Insert(key, item, slidingExpiration, args);
            }
            else
            {
                callIfCached?.Invoke(item);
            }

            return item;
        }

        public T Get<T>(string key, Action<T> callIfCached, Func<T> callIfNotCached, DateTime absoluteExpiration, params object[] args) where T : class
        {
            bool found;
            var item = Get<T>(key, out found, args);
            if (!found)
            {
                item = callIfNotCached();
                Insert(key, item, absoluteExpiration, args);
            }
            else
            {
                callIfCached?.Invoke(item);
            }

            return item;
        }

        public T Get<T>(string key, Func<T> callIfNotCached, TimeSpan slidingExpiration, params object[] args) where T : class
        {
            return Get(key, null, callIfNotCached, slidingExpiration, args);
        }

        public T Get<T>(string key, Func<T> callIfNotCached, DateTime absoluteExpiration, params object[] args) where T : class
        {
            return Get(key, null, callIfNotCached, absoluteExpiration, args);
        }

        /// <summary>
        /// Gets a document by key asynchronously.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Task{T}.</returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task<T> GetAsync<T>(string key, params object[] args)
        {
            if (_instance != null)
            {
                key = CreateKey(key, args);
                var result = await _instance.GetAsync<T>(key);

                if (result.Success)
                {
                    return result.Value;
                }

                if (result.Status == ResponseStatus.KeyNotFound)
                {
                    return default;
                }

                if (result.Exception != null)
                {
                    throw result.Exception;
                }

                throw new CouchbaseException(result, key);
            }

            throw new CouchbaseException("Couchbase GetAsync<T> _instance is null");
        }

        /// <summary>
        /// Updates a document asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task UpdateAsync<T>(string key, T doc, params object[] args)
        {
            if (_instance != null)
            {
                key = CreateKey(key, args);
                var result = await _instance.ReplaceAsync(key, doc);
                if (result.Success)
                {
                    return;
                }
                if (result.Exception != null)
                {
                    // ReSharper disable once ThrowingSystemException
                    throw result.Exception;
                }
                throw new CouchbaseException(result, key);
            }

            throw new CouchbaseException("Couchbase GetAsync<T> _instance is null");
        }

        /// <summary>
        /// Creates a document asynchronously.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="doc">The document.</param>
        /// <returns>T.</returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public T CreateAsync<T>(string key, T doc, params object[] args)
        {
            if (_instance != null)
            {
                key = CreateKey(key, args);
                var result = _instance.Insert(key, doc);
                if (!result.Success)
                {
                    if (result.Exception != null)
                    {
                        throw result.Exception;
                    }
                    throw new CouchbaseException(result, key);
                }

                return result.Value;
            }
            return default;
        }

        /// <summary>
        /// Deletes a document asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Boolean.</returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public bool DeleteAsync(string key, params object[] args)
        {
            if (_instance != null)
            {
                key = CreateKey(key, args);
                var result = _instance.Remove(key);
                if (!result.Success)
                {
                    if (result.Exception != null)
                    {
                        throw result.Exception;
                    }
                    throw new CouchbaseException(result, key);
                }

                return result.Success;
            }

            return false;
        }

        /// <summary>
        /// Gets all documents for a given keyset asynchronously.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns>Task{List{T}}.</returns>
        public async Task<List<T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var tasks = keys.Select(x => _instance.GetAsync<T>(x));
            var results = await Task.WhenAll(tasks);
            return await Task.FromResult(results.Select(x => x.Value).ToList());
        }

        public IEnumerable<T> QueryJson<T>(string query) where T : class
        {
            if (string.IsNullOrEmpty(query)) return Enumerable.Empty<T>();
            if (_instance == null) return Enumerable.Empty<T>();

            var q = new QueryRequest().Statement(query).AdHoc(false);

            var dicObj = _instance.Query<dynamic>(q);

            var result = new List<T>();
            var settings = new DataContractJsonSerializerSettings
            {
                KnownTypes = new Type[] { typeof(T) }
            };
            var serializer = new DataContractJsonSerializer(typeof(T), settings);

            foreach (var item in dicObj.Rows)
            {
                var json = item.ToString();

                if (!string.IsNullOrEmpty(json))
                {
                    var ms = new MemoryStream(Encoding.Default.GetBytes(json));
                    var obj = serializer.ReadObject(ms) as T;
                    ms.Dispose();
                    result.Add(obj);
                }
            }

            return result;
        }

        public void Insert(string key, object data, TimeSpan slidingExpiration, params object[] args)
        {
            if (data != null)
            {
                key = CreateKey(key, args);
                if (_instance != null) _instance.StoreJson(key, data, slidingExpiration);
            }
        }

        public void Insert(string key, object data, DateTime absoluteExpiration, params object[] args)
        {
            if (data != null)
            {
                key = CreateKey(key, args);
                if (_instance != null) _instance.StoreJson(key, data, absoluteExpiration);
            }
        }

        public void Remove(string key, params object[] args)
        {
            key = CreateKey(key, args);
            if (_instance != null) _instance.Remove(key);
        }

        public IEnumerable<string> GetKeys(int limit, string keySetName, string pattern)
        {
            List<string> keys = new List<string>();

            if (_instance != null)
            {
                var query = _instance.CreateQuery(keySetName, pattern).
                    Limit(limit);

                var result = _instance.Query<dynamic>(query);
                foreach (ViewRow<dynamic> row in result.Rows)
                {
                    keys.Add(row.Key);
                }
            }
            return keys;
        }

        public void FlushCache()
        {
            var authToken = Encoding.ASCII.GetBytes($"{CouchbaseConfig.UserName}:{CouchbaseConfig.Password}");

            FlushCache(CouchbaseConfig.Uris[0], CouchbaseConfig.Bucket, Convert.ToBase64String(authToken));
        }

        public void FlushCache(string setName, string pattern, string authToken)
        {
            string url = string.Format("{0}/pools/default/buckets/{1}/controller/doFlush", setName, pattern);
            using (HttpClient client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                }

                _ = client.PostAsync(url, null).Result;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // Dispose of managed resources here.
            if (disposing && _cluster != null)
            {
                _cluster.Dispose();
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.
            _disposed = true;
        }

        private static string CreateKey(string key, params object[] args)
        {
            return (args.Length > 0) ? string.Format(key, args) : key;
        }
    }
}
