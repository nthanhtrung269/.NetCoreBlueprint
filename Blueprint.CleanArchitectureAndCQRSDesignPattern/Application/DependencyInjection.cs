using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Behaviors;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.WriteModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System.Reflection;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Apply the Mediator design pattern for CQRS https://github.com/jbogard/MediatR/wiki
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // Mediatr Pipeline Behaviour: https://github.com/jbogard/MediatR/wiki/Behaviors
            // https://www.codewithmukesh.com/blog/mediatr-pipeline-behaviour/
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = new TypeAdapterConfig();
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            AddMapperConfigurations(config);

            services.AddScoped<ISettingAppService, SettingAppService>();
            services.AddScoped<ITransactionLogService, TransactionLogService>();
            services.AddScoped<IDimensionsUpdaterService, DimensionsUpdaterService>();
            services.AddScoped<IPreGeneratorService, PreGeneratorService>();
            services.AddScoped<ICachingService, CachingService>();
            services.AddScoped<IImageService, ImageService>();

            return services;
        }

        private static void AddMapperConfigurations(TypeAdapterConfig config)
        {
            config.NewConfig<FileDto, BlueprintFile>().PreserveReference(true);
            config.NewConfig<BlueprintFile, FileDto>().PreserveReference(true);
        }
    }
}
