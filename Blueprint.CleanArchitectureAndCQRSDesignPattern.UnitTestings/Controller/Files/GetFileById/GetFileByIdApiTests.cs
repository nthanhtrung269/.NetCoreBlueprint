using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetMultipleFilesByIds;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.UnitTestings.Controller.Files.GetFileById
{
    public class GetFileByIdApiTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IOptions<AssetSettings>> _configuration;
        private readonly Mock<MimeTypeFactory> _mimeTypeFactory;

        public GetFileByIdApiTests()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new Mock<IOptions<AssetSettings>>();
            _mimeTypeFactory = new Mock<MimeTypeFactory>();
        }

        [Fact]
        public async Task Api_ShouldGetFileByIdSuccessfullly()
        {
            // arrange
            var file = new BaseFileDto() { Id = 1, FilePath = "file_path/file_name.jpg" };
            _mediator.Setup(f => f.Send(It.IsAny<GetFileByIdQuery>(), CancellationToken.None))
               .ReturnsAsync(file);

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            var result = await controller.GetFileById((int)file.Id);

            // assert
            _mediator.Verify(sender => sender.Send(It.IsAny<GetFileByIdQuery>(), CancellationToken.None), Times.Once);
            Assert.Equal(result.FilePath, file.FilePath);
        }

        [Fact]
        public async Task Api_ShouldGetMultiFileByIdSuccessfullly()
        {
            // arrange
            var files = new List<BaseFileDto>()
            {
                new BaseFileDto() { Id = 1, FilePath = "file_path/file_name1.jpg" },
                new BaseFileDto() { Id = 2, FilePath = "file_path/file_name2.jpg" },
                new BaseFileDto() { Id = 3, FilePath = "file_path/file_name3.jpg" }
            };

            _mediator.Setup(f => f.Send(It.IsAny<GetMultipleFilesByIdsQuery>(), CancellationToken.None))
               .ReturnsAsync(files);

            // act
            var controller = new FileController(_mediator.Object, _configuration.Object, _mimeTypeFactory.Object);
            var results = await controller.GetMultipleFilesByIds(files.Select(x => x.Id).ToArray());

            // assert
            _mediator.Verify(sender => sender.Send(It.IsAny<GetMultipleFilesByIdsQuery>(), CancellationToken.None), Times.Once);

            for (int i = 0; i < files.Count; i++)
            {
                Assert.Equal(files[0].FilePath, results[0].FilePath);
            }
        }
    }
}
