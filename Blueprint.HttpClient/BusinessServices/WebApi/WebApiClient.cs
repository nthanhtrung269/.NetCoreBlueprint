using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blueprint.HttpClient1.BusinessServices.WebApi
{
    public class WebApiClient
    {
        private readonly ILogger<WebApiClient> _logger;
        protected readonly HttpClient _httpClient;

        protected internal WebApiClient(ILogger<WebApiClient> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets async.
        /// </summary>
        /// <typeparam name="T">The T.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="providerName">The providerName.</param>
        /// <returns>Task{T}.</returns>
        public Task<T> GetAsync<T>(string description, Uri requestUri, CancellationToken cancellationToken, string providerName)
        {
            return CallApiAsync<T>(description, client => CheckForWebException(description, client.GetAsync(requestUri, cancellationToken)), providerName);
        }

        /// <summary>
        /// Posts async.
        /// </summary>
        /// <typeparam name="TRequest">The TRequest.</typeparam>
        /// <typeparam name="TResponse">The TResponse.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="requestUri">The requestUri.</param>
        /// <param name="requestContent">The requestContent.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <param name="dataInterchangeFormat">The dataInterchangeFormat.</param>
        /// <param name="providerName">The providerName.</param>
        /// <param name="jsonSerializerSettings">The jsonSerializerSettings.</param>
        /// <returns>Task{TResponse}.</returns>
        public Task<TResponse> PostAsync<TRequest, TResponse>(string description, Uri requestUri, TRequest requestContent, CancellationToken cancellationToken, DataInterchangeFormat dataInterchangeFormat, string providerName, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var serializedRequestContent = new StringContent("");
            int dataInterchangeFormatInt = Convert.ToInt32(dataInterchangeFormat);

            if (dataInterchangeFormatInt == 0)
            {
                serializedRequestContent = SerializeAsJsonString(requestContent, jsonSerializerSettings);
            }
            else if (dataInterchangeFormatInt == 1)
            {
                serializedRequestContent = SerializeAsXmlString(requestContent);
            }
            else
            {
                throw new ArgumentException("Method parameter 'dataInterchangeFormat' must have a value of 'xml' or 'json'.", nameof(dataInterchangeFormat));
            }

            return CallApiAsync<TResponse>(description, client => CheckForWebException(description, client.PostAsync(requestUri, serializedRequestContent, cancellationToken)), providerName);
        }

        /// <summary>
        /// Creates request.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>FluentUriBuilder.</returns>
        public virtual FluentUriBuilder CreateRequest(string uri, params KeyValuePair<string, object>[] parameters)
        {
            return new FluentUriBuilder(uri);
        }

        private async Task<T> CallApiAsync<T>(string description, Func<HttpClient, Task<HttpResponseMessage>> apiCall, string providerName)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                T value;

                using (HttpResponseMessage response = await apiCall(_httpClient))
                {
                    string valueString = await response.Content.ReadAsStringAsync();
                    timer.Stop();

                    if (typeof(T) == typeof(string))
                    {
                        //Allow string objects to bypass deserialization
                        value = (T)(object)valueString;
                    }
                    else
                    {
                        _logger.LogDebug($"Response from {providerName}: " + valueString);
                        if (valueString.TrimStart().StartsWith("<"))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            // convert string to stream
                            byte[] byteArray = Encoding.UTF8.GetBytes(valueString);
                            MemoryStream stream = new MemoryStream(byteArray);
                            value = (T)serializer.Deserialize(stream);
                        }
                        else
                            value = JsonConvert.DeserializeObject<T>(valueString);
                    }
                }

                return value;
            }
            catch (WebApiException ex) when (!ex.IsTransientError)
            {
                throw;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError($"{description} - Circuit is open, no calls were attempted.", ex);
                throw new BrokenCircuitException($"{description} - Circuit is open, no calls were attempted.", ex);
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogWarning($"HTTP call to {providerName} timed out.", ex);
                throw new TimeoutRejectedException($"HTTP call to {providerName} timed out.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"HTTP call to {providerName} failed.", ex);
                throw;
            }
        }

        private async Task<HttpResponseMessage> CheckForWebException(string description, Task<HttpResponseMessage> response)
        {
            var res = await response;
            if (!res.IsSuccessStatusCode)
            {
                string content = res.Content != null ? await res.Content?.ReadAsStringAsync() : string.Empty;
                string warning = "Call returned with " + description + " statusCode " + res.StatusCode + " and response " + content;
                _logger.LogWarning(warning);
                throw new WebApiException(res.StatusCode, "Call unsuccessful. " + warning);
            }
            return res;
        }

        private StringContent SerializeAsJsonString<T>(T obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            string content = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private StringContent SerializeAsXmlString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                StringContent xmlString = new StringContent(textWriter.ToString(), Encoding.UTF8, "text/xml");

                return xmlString;
            }
        }
    }
}
