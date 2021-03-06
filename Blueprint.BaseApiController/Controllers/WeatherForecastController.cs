using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blueprint.BaseApiController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
            : base(logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return ValidateAndProcessRequestWithGuard(new object(),
                req =>
                {
                    _logger.LogInformation($"Call to {nameof(Get)}");
                    var rng = new Random();
                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                    .ToArray();
                });
        }

        [HttpGet("standard-response")]
        public WeatherForecastViewModel GetStandardResponse(string message)
        {
            return ValidateAndProcessRequest(new { Message = message },
                req =>
                {
                    _logger.LogInformation($"Call to {nameof(Get)}");
                    var rng = new Random();
                    return new WeatherForecastViewModel()
                    {
                        CorrelationId = Guid.NewGuid(),
                        Message = req.Message,
                        Status = true,
                        ErrorCode = ResponseErrorCode.None,
                        WeatherForecasts = Enumerable.Range(1, 5)
                                            .Select(index => new WeatherForecast
                                            {
                                                Date = DateTime.Now.AddDays(index),
                                                TemperatureC = rng.Next(-20, 55),
                                                Summary = Summaries[rng.Next(Summaries.Length)]
                                            })
                                            .ToArray()
                    };
                });
        }
    }
}
