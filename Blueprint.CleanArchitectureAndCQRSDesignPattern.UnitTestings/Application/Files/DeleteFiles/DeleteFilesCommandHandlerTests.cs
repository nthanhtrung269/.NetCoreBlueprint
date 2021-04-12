using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFiles;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Exceptions;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Application.Files.DeleteFiles
{
    public class DeleteFilesCommandHandlerTests
    {
        private readonly DeleteFilesCommandHandler _handler;
        private readonly Mock<IFileRepository> _fileRepositoryMock;
        private readonly Mock<IFileSystemService> _fileSystemServiceMock;

        public DeleteFilesCommandHandlerTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _fileSystemServiceMock = new Mock<IFileSystemService>();
            _handler = new DeleteFilesCommandHandler(_fileRepositoryMock.Object, _fileSystemServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ThrowExceptionGivenNullEventArgument()
        {
            // arrange

            // act

            // assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));

            DeleteFilesCommand deleteFilesCommand = new DeleteFilesCommand(new long[] { });
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(deleteFilesCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldSuccecssDeleteFiles()
        {
            // arrange
            List<BlueprintFile> blueprintFiles = new List<BlueprintFile>();
            blueprintFiles.Add(new BlueprintFile() { Id = 1, FilePath = "file_path/file_name1.jpg" });
            blueprintFiles.Add(new BlueprintFile() { Id = 2, FilePath = "file_path/file_name2.jpg" });
            blueprintFiles.Add(new BlueprintFile() { Id = 3, FilePath = "file_path/file_name3.jpg" });

            long[] ids = blueprintFiles.Select(x => x.Id).ToArray();

            _fileRepositoryMock.Setup(f => f.FindByAsync(It.IsAny<Expression<Func<BlueprintFile, bool>>>()))
            .ReturnsAsync(blueprintFiles);

            // act
            await _handler.Handle(new DeleteFilesCommand(ids), CancellationToken.None);

            // assert
            _fileRepositoryMock.Verify(sender => sender.FindByAsync(It.IsAny<Expression<Func<BlueprintFile, bool>>>()), Times.Once);

            foreach (var file in blueprintFiles)
            {
                _fileSystemServiceMock.Verify(sender => sender.Delete(file.FilePath), Times.Once);
            }

            _fileRepositoryMock.Verify(sender => sender.DeleteFiles(blueprintFiles), Times.Once);
        }
    }
}
