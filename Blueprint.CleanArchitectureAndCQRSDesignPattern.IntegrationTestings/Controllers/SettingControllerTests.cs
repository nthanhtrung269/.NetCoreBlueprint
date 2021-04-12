using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Controllers
{
    public class SettingControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public SettingControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsViewWithCorrectMessage()
        {
            HttpResponseMessage response = await _client.GetAsync("api/setting/value-by-name/IsLoggingDatabase");
            response.EnsureSuccessStatusCode();
            string stringResponse = await response.Content.ReadAsStringAsync();

            Assert.Contains("true", stringResponse);
        }
    }
}
