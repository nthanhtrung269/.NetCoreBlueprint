using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.Polly.WeatherForecastApi
{
    public interface IWeatherForecastApiNotUsingPollyService
    {
        Task<IList<WeatherForecast>> GetWeatherForecastInfo();
    }
}
