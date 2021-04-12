using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Services;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AssetSettings>(configuration.GetSection("AssetSettings"));
            services.Configure<ReadmeSettings>(configuration.GetSection("BuildNote"));
            services.AddDbContext<DBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();

            services.AddScoped<IImageResizerService, ImageResizerService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IFileSystemService, FileSystemService>();
            services.AddSingleton<MimeTypeFactory>();

            return services;
        }
    }
}
