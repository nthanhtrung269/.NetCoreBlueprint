using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.Polly.Models;
using Blueprint.Polly.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.Polly.WeatherForecastApi
{
    public class WeatherForecastApiService : WebApiClient, IWeatherForecastApiService
    {
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<WeatherForecastApiService> _logger;

        public WeatherForecastApiService(IOptions<ApplicationSettings> options,
            ILogger<WeatherForecastApiService> logger,
            HttpClient httpClient) : base(logger, httpClient)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        public async Task<IList<WeatherForecast>> GetWeatherForecastInfo()
        {
            string providerName = _appSettings.WeatherForecastApi.ProviderName;
            string requestUri = $"{_appSettings.WeatherForecastApi.RequestUri}/weatherforecast";
            FluentUriBuilder request = CreateRequest(requestUri);

            IList<WeatherForecast> response = await GetAsync<IList<WeatherForecast>>(
                $"Get WeatherForecast Info {requestUri}",
                request.Uri,
                CancellationToken.None,
                providerName);
            return response;
        }

        public async Task<IList<WeatherForecast>> GetWithServiceUnavailable()
        {
            string providerName = _appSettings.WeatherForecastApi.ProviderName;
            string requestUri = $"{_appSettings.WeatherForecastApi.RequestUri}/weatherforecast/service-unavailable";
            FluentUriBuilder request = CreateRequest(requestUri);

            IList<WeatherForecast> response = await GetAsync<IList<WeatherForecast>>(
                $"Get WeatherForecast Info {requestUri}",
                request.Uri,
                CancellationToken.None,
                providerName);
            return response;
        }
    }
}
