using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Blueprint.HttpClient1.BusinessServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.HttpClient1.Controllers
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
        private readonly IAggregateService _aggregateService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IAggregateService aggregateService)
        {
            _logger = logger;
            _aggregateService = aggregateService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
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
        }

        [HttpPost]
        public async Task Post()
        {
            _logger.LogInformation("ImportOffers started");
            var timer = Stopwatch.StartNew();

            await _aggregateService.ImportOffers();

            timer.Stop();
            _logger.LogInformation($"ImportOffers ended after {timer.ElapsedMilliseconds}ms");
        }
    }
}
