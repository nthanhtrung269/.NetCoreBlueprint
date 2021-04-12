using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Controller.Files.ResizeImage
{
    public class GetResizedImageApiTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<AssetSettings>> _configuration;
        private readonly Mock<MimeTypeFactory> _mimeTypeFactory;

        public GetResizedImageApiTests()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new Mock<IOptions<AssetSettings>>();
            _mimeTypeFactory = new Mock<MimeTypeFactory>();
        }

        [Fact]
        public async Task Api_ShouldGetResizedImageSuccess()
        {
            // arrange
            long id = 1; string imageSize = ""; string filename = "image.jpg";
            _mediator.Setup(f => f.Send(It.IsAny<GetResizedFileByIdQuery>(), CancellationToken.None))
                .ReturnsAsync(filename);

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            var result = await controller.GetResizedImage(id, imageSize);

            // assert
            _mediator.Verify(sender => sender.Send(It.IsAny<GetResizedFileByIdQuery>(), CancellationToken.None), Times.Once);
            var response =  Assert.IsType<PhysicalFileResult>(result);
            Assert.Equal(filename, response.FileName);
        }
    }
}
