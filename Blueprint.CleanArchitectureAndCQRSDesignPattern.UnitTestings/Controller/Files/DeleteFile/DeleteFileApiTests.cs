using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFiles;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Controller.Files.DeleteFile
{
    public class DeleteFileApiTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<AssetSettings>> _configuration;
        private readonly Mock<MimeTypeFactory> _mimeTypeFactory;

        public DeleteFileApiTests()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new Mock<IOptions<AssetSettings>>();
            _mimeTypeFactory = new Mock<MimeTypeFactory>();
        }

        [Fact]
        public async Task Api_ShouldDeleteFileSuccessfullly()
        {
            // arrange

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            var result = await controller.DeleteFile(It.IsAny<int>());

            // assert
            _mediator.Verify(sender => sender.Send(It.IsAny<DeleteFileCommand>(), CancellationToken.None), Times.Once);

            Assert.True(result, "Delete file not success");
        }

        [Fact]
        public async Task Api_ShouldDeleteFilesSuccessfullly()
        {
            // arrange

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            var result = await controller.DeleteFiles(It.IsAny<IList<long>>());

            // assert
            _mediator.Verify(sender => sender.Send(It.IsAny<DeleteFilesCommand>(), CancellationToken.None), Times.Once);

            Assert.True(result, "Delete files not success");
        }
    }
}
