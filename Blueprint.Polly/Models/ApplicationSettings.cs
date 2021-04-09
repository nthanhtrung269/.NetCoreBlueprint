namespace Blueprint.Polly.Models
{
    public class ApplicationSettings
    {
        public string ProductionEnvironment { get; set; }

        public RequestApi WeatherForecastApi { get; set; }
    }

    public class RequestApi
    {
        public string ProviderName { get; set; }

        public string RequestUri { get; set; }

        public string Authorization { get; set; }
    }
}
