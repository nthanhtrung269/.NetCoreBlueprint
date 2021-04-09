using Couchbase;
using Couchbase.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Blueprint.Couchbase
{
    public static class CouchbaseClientExtensions
    {
        public static bool StoreJson(this IBucket client, string key, object value, DateTime? expireAt)
        {
            var json = JsonConvert.SerializeObject(value);
            IOperationResult<string> result;
            if (expireAt.HasValue)
            {
                DateTime epoch = DateTime.Now;
                TimeSpan elapsedTime = expireAt.Value - epoch;
                result = client.Upsert(key, json, elapsedTime);
            }
            else
            {
                result = client.Upsert(key, json);
            }

            return result.Success;
        }

        public static bool StoreJson(this IBucket client, string key, object value, TimeSpan validFor)
        {
            return StoreJson(client, key, value, DateTime.Now.Add(validFor));
        }

        public static T GetJson<T>(this IBucket client, string key) where T : class
        {
            var json = client.Get<string>(key);
            if (json != null && json.Success && !string.IsNullOrEmpty(json.Value))
            {
                var obj = JsonConvert.DeserializeObject<T>(json.Value);
                return obj;
            }
            return null;
        }

        public static Dictionary<string, T> GetMultiJson<T>(this IBucket client, IList<string> keys) where T : class
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
            if (keys.Count > 0)
            {
                // TODO: Use the async overloads that take a list of keys for multi-operations.
                var dicObj = client.Get<dynamic>(keys);

                foreach (var item in dicObj)
                {
                    if (item.Value.Success && !string.IsNullOrEmpty(item.Value.Value))
                    {
                        var json = item.Value.Value.ToString();

                        if (!string.IsNullOrEmpty(json))
                        {
                            var obj = JsonConvert.DeserializeObject<T>(json);

                            if (!result.ContainsKey(item.Key))
                                result.Add(item.Key, obj);
                        }
                    }
                }
            }
            return result;
        }

        public static bool UpdateJson(this IBucket client, string key, object value, Type objType)
        {
            var current = client.Get<string>(key);

            if (!current.Success) throw new CouchbaseException("Item was not found.");

            // Deletes old item 
            var deleteResult = client.Remove(key);
            if (!deleteResult.Success) return false;

            // Inserts to replace with new data
            var json = JsonConvert.SerializeObject(value);
            var result = client.Insert(key, json);
            return result.Success;
        }
    }
}
