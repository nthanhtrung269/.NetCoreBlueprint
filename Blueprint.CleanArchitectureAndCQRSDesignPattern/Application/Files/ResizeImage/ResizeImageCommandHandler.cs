using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.SharedModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile
{
    public class ResizeImageCommandHandler : ICommandHandler<ResizeImageCommand, bool>
    {
        private readonly AssetSettings _configuration;
        private readonly IFileRepository _fileRepository;
        private readonly IImageService _imageService;

        public ResizeImageCommandHandler(IOptions<AssetSettings> configuration,
            IFileRepository fileRepository,
            IImageService imageService)
        {
            _configuration = configuration.Value;
            _fileRepository = fileRepository;
            _imageService = imageService;
        }

        public async Task<bool> Handle(ResizeImageCommand request, CancellationToken cancellationToken)
        {
            var criteria = new ImageResizeCriteria(request.ImageId, request.Query);
            var criteriaValidator = new ImageResizeCriteriaValidator(_configuration);

            Guard.AgainstInvalidArgumentWithMessage("Invalid criteria", criteriaValidator.ValidCriteria(criteria));

            (int? width, int? height) widthHeightPair = criteriaValidator.GetCriteria(criteria);

            var files = _fileRepository.GetFiles(criteria.ImageId, widthHeightPair.width, widthHeightPair.height);

            Guard.AgainstNullOrNotAny("Files", files);

            var originalImage = files.FirstOrDefault(file => file.Id == request.ImageId);

            Guard.AgainstNull("OriginalFile", originalImage);

            //if request has no param -> ask for original image -> return directly to original image
            if (criteria.Empty)
                return true;

            //resized image is always the first one in the list
            var fileResized = files.OrderByDescending(file => file.OriginalId)
                .FirstOrDefault();

            bool canResize = _imageService.CanResize(widthHeightPair, (fileResized.Width, fileResized.Height));
            Guard.AgainstInvalidOperationWithMessage("Cannot Resize this picture with given resolution", canResize);

            bool shouldResize = _imageService.ShouldResize(((int)originalImage.Width, (int)originalImage.Height), widthHeightPair);

            if (!shouldResize)
                return true;

            //file not exist
            if (fileResized.OriginalId == null)
            {
                if ((widthHeightPair.width == null || fileResized.Width == widthHeightPair.width)
                    && (widthHeightPair.height == null ||fileResized.Height == widthHeightPair.height))
                    return true;

                using (var imageData = _imageService.ReadImage(fileResized.FilePath))
                {
                    var oldFileName = Path.GetFileName(fileResized.FilePath);
                    string fileExtension = Path.GetExtension(fileResized.FilePath);
                    var imagePath = Path.GetDirectoryName(fileResized.FilePath);

                    var newFileName = string.Concat(Path.GetFileNameWithoutExtension(oldFileName),
                            widthHeightPair.width == null ? "" : $"_w{widthHeightPair.width}",
                            widthHeightPair.height == null ? "" : $"_h{widthHeightPair.height}",
                            fileExtension);

                    string cloudPath = PathHelper.CombineUrl(imagePath.Replace(_configuration.RootDataFolder, _configuration.CloudDataUrl), string.Empty);

                    using (FileStream outputStream = new FileStream(Path.Combine(imagePath, newFileName), FileMode.CreateNew))
                    {
                        (int? width, int? height) sizeAfterResized =  await _imageService.ResizeImage(imageData, (widthHeightPair.width, widthHeightPair.height), fileExtension, outputStream);

                        var filebuilder = new FileBuilder();
                        var newFile = filebuilder.BuildCloudUrl(cloudPath, newFileName)
                            .BuildFilePath(imagePath, newFileName)
                            .BuildFileInfo(new BlueprintFile
                            {
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = nameof(System),
                                FileType = fileResized.FileType,
                                Height = sizeAfterResized.height,
                                Width = sizeAfterResized.width,
                                OriginalFileName = newFileName,
                                Source = fileResized.Source,
                                OriginalId = fileResized.Id,
                                Extension = fileResized.Extension,
                                CompanyId = fileResized.CompanyId
                            })
                            .Build();

                        await _fileRepository.AddAsync(newFile);
                        await _fileRepository.CommitAsync();

                        return true;
                    }
                }
            }
            return true;
        }
    }
}
