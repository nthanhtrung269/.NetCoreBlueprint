using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Blueprint.Polly.WeatherForecastApi;
using System;
using System.Threading.Tasks;

namespace Blueprint.Polly.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PollyTestingController : BaseApiController
    {
        private readonly ILogger<PollyTestingController> _logger;
        private readonly IWeatherForecastApiService _weatherForecastApiService;
        private readonly IWeatherForecastApiNotUsingPollyService _weatherForecastApiNotUsingPollyService;

        public PollyTestingController(ILogger<PollyTestingController> logger,
            ITransactionLogService transactionLogService,
            ICachingService cachingService,
            IWeatherForecastApiService weatherForecastApiService,
            IWeatherForecastApiNotUsingPollyService weatherForecastApiNotUsingPollyService
            )
            : base(logger, transactionLogService, cachingService)
        {
            _logger = logger;
            _weatherForecastApiService = weatherForecastApiService;
            _weatherForecastApiNotUsingPollyService = weatherForecastApiNotUsingPollyService;
        }

        [HttpGet]
        public async Task<WeatherForecastResponse> Get()
        {
            return await ValidateAndProcessRequestWithGuardAsync(new object(),
                async req =>
                {
                    _logger.LogInformation($"Call to {nameof(Get)}");

                    WeatherForecastResponse weatherForecastResponse = new WeatherForecastResponse()
                    {
                        WeatherForecast = await _weatherForecastApiService.GetWeatherForecastInfo(),
                        ErrorCode = ResponseErrorCode.None,
                        Message = MessageConstants.Ok,
                        Status = true,
                        CorrelationId = Guid.NewGuid()
                    };

                    return weatherForecastResponse;
                });
        }

        [HttpGet("no-polly")]
        public async Task<WeatherForecastResponse> GetWithNoPolly()
        {
            return await ValidateAndProcessRequestWithGuardAsync(new object(),
                async req =>
                {
                    _logger.LogInformation($"Call to {nameof(GetWithNoPolly)}");

                    WeatherForecastResponse weatherForecastResponse = new WeatherForecastResponse()
                    {
                        WeatherForecast = await _weatherForecastApiNotUsingPollyService.GetWeatherForecastInfo(),
                        ErrorCode = ResponseErrorCode.None,
                        Message = MessageConstants.Ok,
                        Status = true,
                        CorrelationId = Guid.NewGuid()
                    };

                    return weatherForecastResponse;
                });
        }

        [HttpGet("service-unavailable")]
        public async Task<WeatherForecastResponse> GetWithServiceUnavailable()
        {
            return await ValidateAndProcessRequestWithGuardAsync(new object(),
                async req =>
                {
                    _logger.LogInformation($"Call to {nameof(GetWithServiceUnavailable)}");

                    WeatherForecastResponse weatherForecastResponse = new WeatherForecastResponse()
                    {
                        WeatherForecast = await _weatherForecastApiService.GetWithServiceUnavailable(),
                        ErrorCode = ResponseErrorCode.None,
                        Message = MessageConstants.Ok,
                        Status = true,
                        CorrelationId = Guid.NewGuid()
                    };

                    return weatherForecastResponse;
                });
        }
    }
}
