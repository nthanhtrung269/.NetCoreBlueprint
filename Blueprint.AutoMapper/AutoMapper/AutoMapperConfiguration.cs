using AutoMapper;
using System.Collections.Generic;

namespace Blueprint.AutoMapper.AutoMapper
{
    /// <summary>
    /// The AutoMapperConfiguration.
    /// </summary>
    public static class AutoMapperConfiguration
    {
        public static List<Profile> RegisterMappings()
        {
            var cfg = new List<Profile>
            {
                new MappingProfile()
            };

            return cfg;
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WeatherForecastDataModel, WeatherForecast>();
        }
    }
}
