using Blueprint.HttpClient1;
using Blueprint.HttpClient1.BusinessServices.Provider1Api;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.HttpClient.IntegrationTestings.BusinessServices.Provider1Api
{
    /// <summary>
    /// Document: Integration tests https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    /// </summary>
    public class Provider1ApiServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public Provider1ApiServiceIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAuthorizationInfo_ShouldRunCorrectly()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                // Arrange
                var Provider1ApiService = scope.ServiceProvider.GetRequiredService<IProvider1ApiService>();

                // Act
                Provider1AuthorizationResponse response = await Provider1ApiService.GetAuthorizationInfo();

                // Assert
                Assert.NotNull(response);
                Assert.NotNull(response.Token);
                Assert.NotNull(response.UserId);
                Assert.NotNull(response.Secret);
                Assert.NotNull(response.AdSessionGuid);
            }
        }
    }
}
