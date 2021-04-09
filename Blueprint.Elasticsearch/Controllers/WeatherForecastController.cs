using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blueprint.Elasticsearch.Controllers
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
        private readonly IElasticsearchClient _elasticsearchClient;
        private readonly ApplicationSettings _appSettings;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IElasticsearchClient elasticsearchClient,
            IOptions<ApplicationSettings> options)
        {
            _logger = logger;
            _elasticsearchClient = elasticsearchClient;
            _appSettings = options.Value;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation($"Call to {nameof(Get)}");
            var rng = new Random();
            var elasticWeatherForecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

            _elasticsearchClient.AddIndex(elasticWeatherForecast, _appSettings.Elasticsearch.IndexName, $"{elasticWeatherForecast.Date}_{elasticWeatherForecast.TemperatureC}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
