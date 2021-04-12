using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProofOfConcept.FireAndForget.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Document: https://dev.to/deanashton/get-your-c-webapi-controller-to-keep-processing-a-request-after-returning-200-ok-38m2
        /// </summary>
        /// <returns>Task{IEnumerable{WeatherForecast}}.</returns>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetWithKeepingConnection()
        {
            try
            {
                _logger.LogInformation($"Call to {nameof(GetWithKeepingConnection)}");

                var result = await Task.Run(() => GetResult());
                return result;
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    await Task.Run(() => SendEmail());
                });
            }
        }

        [HttpGet("without-keep-connection")]
        public async Task<IEnumerable<WeatherForecast>> GetWithoutKeepingConnection()
        {
            _logger.LogInformation($"Call to {nameof(GetWithoutKeepingConnection)}");
            SendEmailFireAndForget();

            var result = await Task.Run(() => GetResult());
            return result;
        }

        private WeatherForecast[] GetResult()
        {
            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();
        }

        private void SendEmail()
        {
            _logger.LogInformation($"Call to {nameof(SendEmail)}");
            Thread.Sleep(10000);

            for (int i = 0; i < 1000; i++)
            {
                _logger.LogInformation($"Do something with task: {i}");
            }

            _logger.LogInformation($"End calling to {nameof(SendEmail)}");
        }

        private void SendEmailFireAndForget()
        {
            Task.Run(() =>
            {
                _logger.LogInformation($"Call to {nameof(SendEmail)}");
                Thread.Sleep(10000);

                for (int i = 0; i < 1000; i++)
                {
                    _logger.LogInformation($"Do something with task: {i}");
                }

                _logger.LogInformation($"End calling to {nameof(SendEmail)}");
            });
        }
    }
}
