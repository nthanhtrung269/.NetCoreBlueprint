using Blueprint.HttpClient1.BusinessServices.Provider1Api;
using Blueprint.HttpClient1.BusinessServices.Provider2Api;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices
{
    public class AggregateService : IAggregateService
    {
        private readonly JobsApplicationSettings _appSettings;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AggregateService> _logger;
        private readonly IProvider2ApiService _Provider2Api;
        private readonly IProvider1ApiService _provider1Api;
        private List<ProductsByProgramRun> _failedProducts;
        private List<ProductsByProgramRun> _failedProductsLoggedToDB;
        private bool _retryFaildedProductsByProgramRun = true;
        private const int PageSize = 50;

        public AggregateService(IOptions<JobsApplicationSettings> options,
            IMemoryCache cache,
            ILogger<AggregateService> logger,
            IProvider2ApiService Provider2Api,
            IProvider1ApiService Provider1Api)
        {
            _appSettings = options.Value;
            _cache = cache;
            _logger = logger;
            _Provider2Api = Provider2Api;
            _provider1Api = Provider1Api;
        }

        /// <summary>
        /// Imports offers.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public async Task<bool> ImportOffers()
        {
            try
            {
                _logger.LogInformation($"{nameof(ImportOffers)} started");
                _failedProducts = new List<ProductsByProgramRun>();
                _failedProductsLoggedToDB = new List<ProductsByProgramRun>();

                // Step 1: Gets DB
                _logger.LogInformation($"Step 1: Gets DB");
                List<ProductsByProgramRun> offerProducts = new List<ProductsByProgramRun>();

                if (!offerProducts.Any())
                {
                    _logger.LogInformation($"{nameof(ImportOffers)} ended - There is no product");
                    return true;
                }

                // Step 2: Calls Apis
                _logger.LogInformation($"Total products from DB {offerProducts.Count}");
                _logger.LogInformation($"Step 2: Calls Apis");

                // Gets Provider1 Authorization info
                Provider1AuthorizationResponse provider1AuthorizationResponse = await _provider1Api.GetAuthorizationInfo();

                // Step 3: Aggregates data
                _logger.LogInformation($"Step 3: Aggregates data");
                for (int i = 0; i * PageSize <= offerProducts.Count; i++)
                {
                    _logger.LogInformation($"Start SearchProductsWithPageSize with PageIndex: {i}, PageSize: {PageSize}");
                    _ = await SearchProductsWithPageSize(offerProducts.Skip(i * PageSize).Take(PageSize).ToList(), provider1AuthorizationResponse.Token);

                    // Step 4: Insert Elasticsearch
                    _logger.LogInformation($"Step 4: Insert Elasticsearch");
                }

                // Step 5: Process failed offerProducts
                _logger.LogInformation($"Step 5: Process failed offerProducts");
                if (_retryFaildedProductsByProgramRun)
                {
                    _retryFaildedProductsByProgramRun = false;

                    if (_failedProducts.Count > 0)
                    {
                        Thread.Sleep(1000 * 60);
                        foreach (ProductsByProgramRun offerProduct in _failedProducts)
                        {
                            // Do something
                        }
                    }
                }

                // Step 6: Updates DB
                _logger.LogInformation($"Step 6: Updates DB");

                _logger.LogInformation($"{nameof(ImportOffers)} ended");
                _logger.LogInformation($"Total failed products logged to DB {_failedProductsLoggedToDB.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(ImportOffers)}", ex);
                throw;
            }

            return true;
        }

        private async Task<List<ProductResponseModel>> SearchProductsWithPageSize(List<ProductsByProgramRun> offerProducts, string token)
        {
            var productReponses = new List<ProductResponseModel>();
            var tasks = new List<Task<List<ProductResponseModel>>>();

            tasks.Add(GetProvider1ProductResponsesAsync(offerProducts, token));
            tasks.Add(_Provider2Api.SearchProducts(offerProducts));

            var completionTask = Task.WhenAll(tasks);

            try
            {
                await completionTask;
            }
            catch
            {
                _logger.LogError(string.Join(", ", completionTask.Exception.Flatten().InnerExceptions.Select(e => e.Message)));
            }

            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    List<ProductResponseModel> productResponses = task.Result;
                    if (productResponses != null)
                    {
                        productReponses.AddRange(productResponses);
                    }
                }
            }

            // Gets Provider2 products
            List<ProductResponseModel> provider2Products = productReponses.Where(p => p.ProductType == ProductType.Provider2).ToList();

            // Sync with Provider1 product info
            foreach (ProductResponseModel provider2Product in provider2Products)
            {
                ProductResponseModel provider1Product = productReponses.FirstOrDefault(p => p.ProductType == ProductType.Provider1 && p.ProductCode == provider2Product.ProductCode);

                if (provider1Product != null)
                {
                    provider2Product.Provider1ImageUrl = provider1Product.Provider1ImageUrl;
                }

                // Adds to failed products
                if (string.IsNullOrEmpty(provider2Product.Provider1ImageUrl) && string.IsNullOrEmpty(provider2Product.Provider2ImageUrl))
                {
                    if (_retryFaildedProductsByProgramRun)
                    {
                        _failedProducts.Add(provider2Product.ProductsByProgramRun);
                    }
                    else
                    {
                        _failedProductsLoggedToDB.Add(provider2Product.ProductsByProgramRun);
                    }
                }
            }

            return provider2Products;
        }

        private async Task<List<ProductResponseModel>> GetProvider1ProductResponsesAsync(List<ProductsByProgramRun> offerProducts, string token)
        {
            var productReponses = new List<ProductResponseModel>();

            var tasks = new List<Task<ProductResponseModel>>();

            foreach (ProductsByProgramRun offerProduct in offerProducts)
            {
                try
                {
                    if (!_cache.TryGetValue(offerProduct.ProductCode, out ProductResponseModel productResponseDTO))
                    {
                        tasks.Add(_provider1Api.SearchProduct(offerProduct, token));
                    }
                    else
                    {
                        _logger.LogInformation($"Gets product from cache with ProductCode={offerProduct.ProductCode}");
                        productReponses.Add(productResponseDTO);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in Getting MemoryCache", ex);
                }
            }

            var completionTask = Task.WhenAll(tasks);

            try
            {
                await completionTask;
            }
            catch
            {
                _logger.LogError(string.Join(", ", completionTask.Exception.Flatten().InnerExceptions.Select(e => e.Message)));
            }

            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ProductResponseModel provider1ProductsResponse = task.Result;
                    if (provider1ProductsResponse != null)
                    {
                        productReponses.Add(provider1ProductsResponse);

                        // Cache response objects from API
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60 * 60 * 1));
                        _cache.Set(provider1ProductsResponse.ProductCode, provider1ProductsResponse, cacheEntryOptions);
                    }
                }
            }

            return productReponses;
        }
    }
}
