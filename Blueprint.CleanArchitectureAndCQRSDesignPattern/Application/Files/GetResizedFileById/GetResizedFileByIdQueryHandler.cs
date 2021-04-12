using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.SharedModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById
{
    public class GetResizedFileByIdQueryHandler : IQueryHandler<GetResizedFileByIdQuery, string>
    {
        private readonly AssetSettings _configuration;
        private readonly IImageService _imageService;
        private readonly IFileReadRepository _fileReadRepository;

        public GetResizedFileByIdQueryHandler(IOptions<AssetSettings> configuration,
            IImageService imageService,
            IFileReadRepository fileReadRepository)
        {
            _configuration = configuration.Value;
            _imageService = imageService;
            _fileReadRepository = fileReadRepository;
        }

        private static class Constants
        {
            public const string File = "File";
            public const string OriginalFile = "Original File";
        }

        public async Task<string> Handle(GetResizedFileByIdQuery request, CancellationToken cancellationToken)
        {
            var criteria = new ImageResizeCriteria(request.ImageId, request.Query);
            var criteriaValidator = new ImageResizeCriteriaValidator(_configuration);

            Guard.AgainstInvalidArgumentWithMessage("Invalid criteria", criteriaValidator.ValidCriteria(criteria));

            (int? width, int? height) widthHeightPair = criteriaValidator.GetCriteria(criteria);

            var fileDtos = await _fileReadRepository.GetAllImageOfIdAsync(request.ImageId, widthHeightPair.width, widthHeightPair.height);

            Guard.AgainstNullOrNotAny(Constants.File, fileDtos);

            BaseFileDto fileFound;

            fileFound = fileDtos.FirstOrDefault(file => (widthHeightPair.width == null || file.Width == widthHeightPair.width)
                                                        && (widthHeightPair.height == null || file.Height == widthHeightPair.height) && !criteria.Empty);
            if(fileFound != null)
            {
                return fileFound.FilePath;
            }

            BaseFileDto originalFile = fileDtos.FirstOrDefault(file => file.Id == criteria.ImageId);

            Guard.AgainstNull(Constants.OriginalFile, originalFile);

            bool shoudResize = _imageService.ShouldResize(((int)originalFile.Width, (int)originalFile.Height), widthHeightPair);

            if (criteria.Empty || !shoudResize)
            {
                fileFound = originalFile;
            }

            return fileFound?.FilePath;
        }
    }
}
