using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetMultipleFilesByIds
{
    public class GetMultipleFilesByIdsQueryHandler : IQueryHandler<GetMultipleFilesByIdsQuery, IList<BaseFileDto>>
    {
        private readonly IFileReadRepository _fileReadRepository;
        private readonly AssetSettings _configuration;

        public GetMultipleFilesByIdsQueryHandler(IFileReadRepository fileReadRepository, IOptions<AssetSettings> config)
        {
            _fileReadRepository = fileReadRepository;
            _configuration = config.Value;
        }

        public async Task<IList<BaseFileDto>> Handle(GetMultipleFilesByIdsQuery request, CancellationToken cancellationToken)
        {
            List<BaseFileDto> fileDtos = (await _fileReadRepository.GetResizedFileByIdQueryAsync(request.Ids)).ToList();

            if (fileDtos != null)
            {
                foreach (var file in fileDtos)
                {
                    if (file != null)
                    {
                        string rootDataFolder = _configuration.RootDataFolder;
                        string cloudDataUrl = _configuration.CloudDataUrl;

                        string fileName = file.FilePath.Replace(rootDataFolder, string.Empty);
                        file.CloudUrl = PathHelper.CombineUrl(cloudDataUrl, fileName);
                    }
                }
            }

            return fileDtos;
        }
    }
}
