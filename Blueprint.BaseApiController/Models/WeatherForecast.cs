using System.Collections.Generic;

namespace Blueprint.BaseApiController
{
    public class WeatherForecastViewModel : BaseResponseObject
    {
        public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
    }
}
