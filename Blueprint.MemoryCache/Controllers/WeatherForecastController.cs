using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blueprint.Nlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMemoryCache _cache;
        private const string WeatherForecastCacheKey = "WeatherForecastCacheKey";

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation($"Call to {nameof(Get)}");
            if (_cache.TryGetValue(WeatherForecastCacheKey, out WeatherForecast[] weatherForecasts))
            {
                return weatherForecasts;
            }

            var rng = new Random();
            WeatherForecast[] newWeatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            _cache.Set(WeatherForecastCacheKey, newWeatherForecasts, TimeSpan.FromMinutes(5));
            return newWeatherForecasts;
        }
    }
}
