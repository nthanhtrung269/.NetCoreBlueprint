using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetFileById
{
    public class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, BaseFileDto>
    {
        private readonly IFileReadRepository _fileReadRepository;

        public GetFileByIdQueryHandler(IFileReadRepository fileReadRepository)
        {
            _fileReadRepository = fileReadRepository;
        }

        public async Task<BaseFileDto> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(nameof(GetFileByIdQuery), request);
            var fileDto = await _fileReadRepository.GetFileByIdQuery(request.Id);
            return fileDto;
        }
    }
}
