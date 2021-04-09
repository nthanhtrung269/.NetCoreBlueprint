using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Blueprint.BuilderDesignPattern
{
    /// <summary>
    /// Helper class creating a Uri using a fluent API.
    /// </summary>
    public class FluentUriBuilder
    {
        private readonly IList<string> _queryPathParams = new List<string>();
        private readonly NameValueCollection _queryStringParams = new NameValueCollection();
        private readonly string _uri;

        public FluentUriBuilder(string uri)
        {
            _uri = uri;
        }

        public Uri Uri
        {
            get
            {
                // Build query string
                var pairs = from key in _queryStringParams.AllKeys
                            from value in _queryStringParams.GetValues(key)
                            select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}";
                var queryString = string.Join("&", pairs);
                var queryPath = string.Join("/", _queryPathParams);
                if (!string.IsNullOrEmpty(queryPath))
                {
                    queryPath = queryPath.Insert(0, "/");
                }

                // Build complete uri
                var builder = new UriBuilder(_uri + queryPath);
                builder.Query = queryString;

                return builder.Uri;
            }
        }

        public string QueryString => string.Join(" | ", _queryStringParams.AllKeys.Select(key => $"{key} {_queryStringParams[key]}"));

        public FluentUriBuilder AddQueryStringParam(string name, object value)
        {
            if (value != null)
            {
                _queryStringParams.Add(name, value.ToString());
            }

            return this;
        }

        public FluentUriBuilder AppendToUri(object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                _queryPathParams.Add(value.ToString());
            }

            return this;
        }
    }
}
