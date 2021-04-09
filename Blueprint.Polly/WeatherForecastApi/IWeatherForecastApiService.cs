using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.Polly.WeatherForecastApi
{
    public interface IWeatherForecastApiService
    {
        Task<IList<WeatherForecast>> GetWeatherForecastInfo();

        Task<IList<WeatherForecast>> GetWithServiceUnavailable();
    }
}
