using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Blueprint.AdapterDesignPattern.Models;
using Blueprint.AdapterDesignPattern.Services.WebApiClientServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    public class FirstDataApiAdaptee : WebApiClient, IFirstDataApiAdaptee
    {
        private readonly MerchantAccountSettings _appSettings;
        private readonly ILogger<FirstDataApiAdaptee> _logger;

        private static class PathConstants
        {
            public const string Authorize = "/auth";
            public const string Capture = "/capture";
            public const string Void = "/void";
            public const string Refund = "/refund";
            public const string Inquire = "/inquire/{{retref}}/{{merchid}}";
        }

        public FirstDataApiAdaptee(IOptions<MerchantAccountSettings> options,
            ILogger<FirstDataApiAdaptee> logger,
            HttpClient httpClient) : base(logger, httpClient)
        {
            _appSettings = options.Value;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", _appSettings.FirstDataApi.Authorization);
        }

        /// <summary>
        /// Does authorize method.
        /// </summary>
        /// <returns>Task{FirstDataAuthorizeResponse}.</returns>
        public async Task<FirstDataAuthorizeResponse> Authorize(FirstDataAuthorizeRequest request)
        {
            // Logs request data
            _logger.LogInformation($"Call to {nameof(Authorize)} with request: {JsonConvert.SerializeObject(request)}.");

            string providerName = _appSettings.FirstDataApi.ProviderName;
            string requestUri = $"{_appSettings.FirstDataApi.ApiUrl}{PathConstants.Authorize}";
            FluentUriBuilder httpRequest = CreateRequest(requestUri);

            FirstDataAuthorizeResponse response = await PutAsync<FirstDataAuthorizeRequest, FirstDataAuthorizeResponse>(
                $"Put Authorize Request {requestUri}",
                httpRequest.Uri,
                request,
                CancellationToken.None,
                providerName);

            // Logs response data
            _logger.LogInformation($"Call to {nameof(Authorize)} with response: {JsonConvert.SerializeObject(response)}.");
            return response;
        }

        /// <summary>
        /// Does capture method.
        /// </summary>
        /// <returns>Task{FirstDataCaptureResponse}.</returns>
        public async Task<FirstDataCaptureResponse> Capture(FirstDataCaptureRequest request)
        {
            // Logs request data
            _logger.LogInformation($"Call to {nameof(Capture)} with request: {JsonConvert.SerializeObject(request)}.");

            string providerName = _appSettings.FirstDataApi.ProviderName;
            string requestUri = $"{_appSettings.FirstDataApi.ApiUrl}{PathConstants.Capture}";
            FluentUriBuilder httpRequest = CreateRequest(requestUri);

            FirstDataCaptureResponse response = await PutAsync<FirstDataCaptureRequest, FirstDataCaptureResponse>(
                $"Put Capture Request {requestUri}",
                httpRequest.Uri,
                request,
                CancellationToken.None,
                providerName);

            // Logs response data
            _logger.LogInformation($"Call to {nameof(Capture)} with response: {JsonConvert.SerializeObject(response)}.");
            return response;
        }

        /// <summary>
        /// Does void method.
        /// </summary>
        /// <returns>Task{FirstDataVoidResponse}.</returns>
        public async Task<FirstDataVoidResponse> Void(FirstDataVoidRequest request)
        {
            // Logs request data
            _logger.LogInformation($"Call to {nameof(Void)} with request: {JsonConvert.SerializeObject(request)}.");

            string providerName = _appSettings.FirstDataApi.ProviderName;
            string requestUri = $"{_appSettings.FirstDataApi.ApiUrl}{PathConstants.Void}";
            FluentUriBuilder httpRequest = CreateRequest(requestUri);

            FirstDataVoidResponse response = await PutAsync<FirstDataVoidRequest, FirstDataVoidResponse>(
                $"Put Void Request {requestUri}",
                httpRequest.Uri,
                request,
                CancellationToken.None,
                providerName);

            // Logs response data
            _logger.LogInformation($"Call to {nameof(Void)} with response: {JsonConvert.SerializeObject(response)}.");
            return response;
        }

        /// <summary>
        /// Does refund method.
        /// </summary>
        /// <returns>Task{FirstDataRefundResponse}.</returns>
        public async Task<FirstDataRefundResponse> Refund(FirstDataRefundRequest request)
        {
            // Logs request data
            _logger.LogInformation($"Call to {nameof(Refund)} with request: {JsonConvert.SerializeObject(request)}.");

            string providerName = _appSettings.FirstDataApi.ProviderName;
            string requestUri = $"{_appSettings.FirstDataApi.ApiUrl}{PathConstants.Refund}";
            FluentUriBuilder httpRequest = CreateRequest(requestUri);

            FirstDataRefundResponse response = await PutAsync<FirstDataRefundRequest, FirstDataRefundResponse>(
                $"Put Refund Request {requestUri}",
                httpRequest.Uri,
                request,
                CancellationToken.None,
                providerName);

            // Logs response data
            _logger.LogInformation($"Call to {nameof(Refund)} with response: {JsonConvert.SerializeObject(response)}.");
            return response;
        }
    }
}
