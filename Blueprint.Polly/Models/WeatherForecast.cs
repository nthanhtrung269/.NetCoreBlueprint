using System.Collections.Generic;

namespace Blueprint.Polly
{
    public class WeatherForecastViewModel : BaseResponseObject
    {
        public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
    }
}
