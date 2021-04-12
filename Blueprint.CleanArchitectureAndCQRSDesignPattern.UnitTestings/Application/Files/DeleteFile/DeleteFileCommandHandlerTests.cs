using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Application.Files.DeleteFile
{
    public class DeleteFileCommandHandlerTests
    {
        private readonly DeleteFileCommandHandler _handler;
        private readonly Mock<IFileRepository> _fileRepositoryMock;
        private readonly Mock<IFileSystemService> _fileSystemServiceMock;

        public DeleteFileCommandHandlerTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _fileSystemServiceMock = new Mock<IFileSystemService>();
            _handler = new DeleteFileCommandHandler(_fileRepositoryMock.Object, _fileSystemServiceMock.Object);
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
        public async Task Handle_ShouldDeleteFileSuccessfully_WhenFileIdIsValid()
        {
            // Arrange
            _fileRepositoryMock.Setup(f => f.GetByIdAsync(1)).ReturnsAsync(new BlueprintFile() { FilePath = "file_path/file_name.jpg" });
            _fileRepositoryMock.Setup(f => f.DeleteAsync(It.IsAny<BlueprintFile>()));
            _fileRepositoryMock.Setup(f => f.CommitAsync());
            _fileSystemServiceMock.Setup(f => f.Delete("file_path/file_name.jpg"));

            // Act
            await _handler.Handle(new DeleteFileCommand(1), CancellationToken.None);

            // Assert
            _fileRepositoryMock.Verify(sender => sender.GetByIdAsync(1), Times.Once);
            _fileSystemServiceMock.Verify(sender => sender.Delete("file_path/file_name.jpg"), Times.Once);
            _fileRepositoryMock.Verify(sender => sender.DeleteAsync(It.IsAny<BlueprintFile>()), Times.Once);
            _fileRepositoryMock.Verify(sender => sender.CommitAsync(), Times.Once);
        }
    }
}
