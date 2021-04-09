using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.HttpClient1.BusinessServices.WebApi;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices.Provider1Api
{
    public class Provider1ApiService : WebApiClient, IProvider1ApiService
    {
        private readonly JobsApplicationSettings _appSettings;
        private readonly ILogger<Provider1ApiService> _logger;

        public Provider1ApiService(IOptions<JobsApplicationSettings> options,
            ILogger<Provider1ApiService> logger,
            HttpClient httpClient) : base(logger, httpClient)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets authorization info.
        /// </summary>
        /// <returns>Task{Provider1AuthorizationResponse}.</returns>
        public async Task<Provider1AuthorizationResponse> GetAuthorizationInfo()
        {
            string providerName = _appSettings.AuthorizationProvider1Api.ProviderName;
            string requestUri = _appSettings.AuthorizationProvider1Api.RequestUri;
            FluentUriBuilder request = CreateRequest(requestUri);

            RemoveDefaultRequestHeaders();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.mywebgrocer.session"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", _appSettings.AuthorizationProvider1Api.Authorization);

            Provider1AuthorizationResponse response = await PostAsync<Provider1AuthorizationRequest, Provider1AuthorizationResponse>(
                $"Get Authorization Info {requestUri}",
                request.Uri,
                new Provider1AuthorizationRequest(),
                CancellationToken.None,
                DataInterchangeFormat.Json,
                providerName);
            return response;
        }

        /// <summary>
        /// Searchs product.
        /// </summary>
        /// <param name="productsByProgramRun">The productsByProgramRun.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task{ProductResponseModel}.</returns>
        public async Task<ProductResponseModel> SearchProduct(ProductsByProgramRun productsByProgramRun, string token)
        {
            try
            {
                string providerName = _appSettings.ProductProvider1Api.ProviderName;
                string requestUri = _appSettings.ProductProvider1Api.RequestUri;
                FluentUriBuilder request = CreateRequest(requestUri);

                request.AddQueryStringParam("skip", "0");
                request.AddQueryStringParam("take", "20");
                request.AddQueryStringParam("isMember", "True");
                request.AddQueryStringParam("q", productsByProgramRun.ProductCode);

                RemoveDefaultRequestHeaders();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.mywebgrocer.product-list+json"));
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);

                var response = await GetAsync<Provider1ProductsResponse>($"SearchProducts from {request.Uri}", request.Uri, CancellationToken.None, providerName);
                ProductResponseModel productResponseDTO = ConvertToProductResponseDTO(productsByProgramRun, response);

                return productResponseDTO;
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(SearchProduct)} Unknown error encountered when reading an object ex {e}");
            }

            return new ProductResponseModel()
            {
                PromotionId = productsByProgramRun.PromotionId.ToString(),
                OfferId = productsByProgramRun.OfferId.ToString(),
                ProductCode = productsByProgramRun.ProductCode,
                Name = productsByProgramRun.Name,
                ProductsByProgramRun = productsByProgramRun,
                ProductType = ProductType.Provider1
            };
        }

        private ProductResponseModel ConvertToProductResponseDTO(ProductsByProgramRun productsByProgramRun, Provider1ProductsResponse provider1ProductsResponse)
        {
            string provider1ImageUrl = provider1ProductsResponse?.Products?.Any() == true
                            ? provider1ProductsResponse.Products[0].ImageLinks?.FirstOrDefault(i => i.Rel == "thumbnail").Uri
                            : string.Empty;

            return new ProductResponseModel()
            {
                PromotionId = productsByProgramRun.PromotionId.ToString(),
                OfferId = productsByProgramRun.OfferId.ToString(),
                ProductCode = productsByProgramRun.ProductCode,
                Name = productsByProgramRun.Name,
                Provider2ImageUrl = string.Empty,
                Provider1ImageUrl = provider1ImageUrl,
                ProductType = ProductType.Provider1
            };
        }

        private void RemoveDefaultRequestHeaders()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }
    }
}
