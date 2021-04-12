using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Infrastructure
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
            });
        }
    }
}
