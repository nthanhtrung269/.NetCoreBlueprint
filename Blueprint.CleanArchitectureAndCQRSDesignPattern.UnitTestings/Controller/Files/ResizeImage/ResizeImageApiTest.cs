using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Controller.Files.ResizeImage
{
    public class ResizeImageApiTest
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<AssetSettings>> _configuration;
        private readonly Mock<MimeTypeFactory> _mimeTypeFactory;

        public ResizeImageApiTest()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new Mock<IOptions<AssetSettings>>();
            _mimeTypeFactory = new Mock<MimeTypeFactory>();
        }

        [Fact]
        public async Task Api_ShouldResizeImageSuccess()
        {
            // arrange
            long id = 1; string imageSize = "";
            _mediator.Setup(f => f.Send(It.IsAny<ResizeImageCommand>(), CancellationToken.None))
                .ReturnsAsync(true);

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            bool result = await controller.ResizeImage(id, imageSize);

            // assert
            Assert.True(result);
        }
    }
}
