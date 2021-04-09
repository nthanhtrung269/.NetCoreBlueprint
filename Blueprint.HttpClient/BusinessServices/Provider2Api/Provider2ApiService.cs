using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Blueprint.HttpClient1.BusinessServices.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices.Provider2Api
{
    public class Provider2ApiService : WebApiClient, IProvider2ApiService
    {
        private readonly JobsApplicationSettings _appSettings;
        private readonly ILogger<Provider2ApiService> _logger;

        public Provider2ApiService(IOptions<JobsApplicationSettings> options,
            ILogger<Provider2ApiService> logger,
            HttpClient httpClient) : base(logger, httpClient)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Searchs products.
        /// </summary>
        /// <param name="productsByProgramRuns">The productsByProgramRuns.</param>
        /// <returns>Task{List{ProductResponseModel}}.</returns>
        public async Task<List<ProductResponseModel>> SearchProducts(List<ProductsByProgramRun> productsByProgramRuns)
        {
            try
            {
                string providerName = _appSettings.ProductProvider2Api.ProviderName;
                string requestUri = _appSettings.ProductProvider2Api.RequestUri;
                FluentUriBuilder request = CreateRequest(requestUri);

                var provider2Request = new Provider2Request
                {
                    CustomerId = 832,
                    PageNum = 0,
                    PageSize = productsByProgramRuns.Count,
                    SortOrder = new List<ProductSortOrderRequestDto>()
                    {
                        new ProductSortOrderRequestDto()
                        {
                            Position = 1,
                            SortColumn = "department",
                            SortDirection = "ASC"
                        }
                    },
                    ItemId = new List<int>(),
                    ItemNum = new List<int>(),
                    UpcList = productsByProgramRuns.Select(p => p.ProductCode).ToList(),
                    BrandName = new List<string>(),
                    DepartmentId = new List<int>(),
                    SubDept1Id = new List<int>(),
                    SubDept2Id = new List<int>(),
                    TprType = "",
                    PromoInd = "",
                    Status = "A",
                    CustomFields = new List<CustomFieldsDto>(),
                    PrimaryBarcode = "Y",
                    DisplayMhSummary = true,
                    DisplayAttributeSummary = true,
                    DisplayBrandSummary = true
                };

                Provider2Response response = await PostAsync<Provider2Request, Provider2Response>(
                    $"Search Products {requestUri}",
                    request.Uri,
                    provider2Request,
                    CancellationToken.None,
                    DataInterchangeFormat.Json,
                    providerName,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                List<ProductResponseModel> productResponses = ConvertToProductResponses(productsByProgramRuns, response);

                return productResponses;
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(SearchProducts)} Unknown error encountered when reading an object ex {e}");
            }

            return ConvertToProductResponses(productsByProgramRuns, null);
        }

        private List<ProductResponseModel> ConvertToProductResponses(List<ProductsByProgramRun> productsByProgramRuns, Provider2Response response)
        {
            var productResponseDTOs = new List<ProductResponseModel>();

            foreach (ProductsByProgramRun productsByProgramRun in productsByProgramRuns)
            {
                Provider2Content provider2Content = GetProvider2Content(response, productsByProgramRun.ProductCode);

                productResponseDTOs.Add(new ProductResponseModel()
                {
                    PromotionId = productsByProgramRun.PromotionId.ToString(),
                    OfferId = productsByProgramRun.OfferId.ToString(),
                    ProductCode = productsByProgramRun.ProductCode,
                    Name = productsByProgramRun.Name,
                    Provider2ImageUrl = provider2Content?.ImageUrl,
                    Provider1ImageUrl = string.Empty,
                    ProductsByProgramRun = productsByProgramRun,
                    ProductType = ProductType.Provider2
                });
            }

            return productResponseDTOs;
        }

        private Provider2Content GetProvider2Content(Provider2Response response, string productCode)
        {
            if (response?.Page?.Content?.Any() == true)
            {
                return response.Page.Content.FirstOrDefault(c => c.Upc == productCode);
            }

            return null;
        }
    }
}
