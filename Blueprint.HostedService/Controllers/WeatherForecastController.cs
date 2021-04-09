using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Blueprint.HostedService.Models;
using Blueprint.HostedService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.HostedService.Controllers
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
        private readonly IFileService _fileService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
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

        [HttpGet("get-file/{id}")]
        public async Task<BaseFileDto> GetFileAsync(long id)
        {
            _logger.LogInformation($"Call to {nameof(GetFileAsync)}");
            return await _fileService.GetFileByIdQuery(id);
        }
    }
}
