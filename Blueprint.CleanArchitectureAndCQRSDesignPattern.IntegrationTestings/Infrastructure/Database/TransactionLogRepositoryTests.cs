using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Infrastructure.Database
{
    public class TransactionLogRepositoryTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public TransactionLogRepositoryTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetByIdAsync_ShouldRunCorrectly()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                // Arrange
                var fileRepository = scope.ServiceProvider.GetRequiredService<ITransactionLogRepository>();

                // Act
                var result = await fileRepository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal(1, result.TransactionLogId);
            }
        }
    }
}
