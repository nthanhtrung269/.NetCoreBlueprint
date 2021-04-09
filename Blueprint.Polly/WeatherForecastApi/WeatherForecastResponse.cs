using System;
using System.Collections.Generic;

namespace Blueprint.Polly.WeatherForecastApi
{
    public class WeatherForecastResponse : BaseResponseObject
    {
        public IList<WeatherForecast> WeatherForecast { get; set; }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
