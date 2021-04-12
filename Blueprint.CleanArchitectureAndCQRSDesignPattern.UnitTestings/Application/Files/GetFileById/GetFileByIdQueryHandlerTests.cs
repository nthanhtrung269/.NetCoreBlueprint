using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Application.Files.GetFileById
{
    public class GetFileByIdQueryHandlerTests
    {
        private readonly GetFileByIdQueryHandler _handler;
        private readonly Mock<IFileReadRepository> _fileReadRepositoryMock;

        public GetFileByIdQueryHandlerTests()
        {
            _fileReadRepositoryMock = new Mock<IFileReadRepository>();
            _handler = new GetFileByIdQueryHandler(_fileReadRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ThrowsExceptionGivenNullEventArgument()
        {
            // Arrange

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_GetFileByIdQuerySuccessfully_WhenFileIdIsValid()
        {
            // Arrange
            _fileReadRepositoryMock.Setup(f => f.QuerySingleOrDefaultAsync<BaseFileDto>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(new BaseFileDto() { Id = 1, FilePath = "file_path/file_name.jpg" });

            // Act
            BaseFileDto baseFileDto = await _handler.Handle(new GetFileByIdQuery(1), CancellationToken.None);

            // Assert
            Assert.Equal(1, baseFileDto.Id);
            Assert.Equal("file_path/file_name.jpg", baseFileDto.FilePath);
        }
    }
}
