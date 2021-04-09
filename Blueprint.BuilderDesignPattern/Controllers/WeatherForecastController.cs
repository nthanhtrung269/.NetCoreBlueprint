using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blueprint.BuilderDesignPattern.Controllers
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

        [HttpGet("get-uri")]
        public string GetUri()
        {
            FluentUriBuilder request = new FluentUriBuilder("https://dev1-api.net/v6/asset/api/");
            return request.AddQueryStringParam("skip", "0")
                .AddQueryStringParam("take", "20")
                .AddQueryStringParam("isMember", "True")
                .Uri
                .ToString();
        }

        [HttpGet("get-file")]
        public string GetFile()
        {
            string cloudPath = "/cloud-path/";
            string newFileName = "test.jpg";
            string imagePath = "/image-path/";
            var filebuilder = new FileBuilder();
            var newFile = filebuilder.BuildCloudUrl(cloudPath, newFileName)
                            .BuildFilePath(imagePath, newFileName)
                            .BuildFileInfo(new ImageFile
                            {
                                FileType = "Product",
                                Height = 100,
                                Width = 150,
                                OriginalFileName = newFileName,
                                Source = "source",
                                OriginalId = 123,
                                Extension = ".jpg",
                                CompanyId = "111"
                            })
                            .Build();

            return JsonConvert.SerializeObject(newFile);
        }
    }
}
