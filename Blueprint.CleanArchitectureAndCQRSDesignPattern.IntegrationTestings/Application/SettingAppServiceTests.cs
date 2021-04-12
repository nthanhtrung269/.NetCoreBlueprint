using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Application
{
    public class SettingAppServiceTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public SettingAppServiceTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetSettingValueByNameAsync_ShouldRunCorrectly()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                // Arrange
                var settingAppService = scope.ServiceProvider.GetRequiredService<ISettingAppService>();

                // Act
                var result = await settingAppService.GetSettingValueByNameAsync("IsLoggingDatabase");

                // Assert
                Assert.Null(result);
            }
        }
    }
}
