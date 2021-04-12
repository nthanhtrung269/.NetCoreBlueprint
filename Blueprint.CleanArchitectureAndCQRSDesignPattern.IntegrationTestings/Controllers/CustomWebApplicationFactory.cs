using Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Controllers
{
    /// <summary>
    /// Document: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    /// <typeparam name="TStartup">The TStartup.</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DBContext>));

                services.Remove(dbContextDescriptor);

                services.AddDbContext<DBContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DBContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        SeedData.PopulateTestData(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            $"database with test messages. Error: {ex.Message}");
                    }

                    logger.LogInformation("DBContext using in memory database...");
                }
            });
        }
    }
}
