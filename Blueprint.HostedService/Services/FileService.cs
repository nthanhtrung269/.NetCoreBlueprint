using Blueprint.HostedService.Dapper;
using Blueprint.HostedService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.HostedService.Services
{
    public class FileService : IFileService
    {
        private readonly IFileReadRepository _fileReadRepository;

        public FileService(IFileReadRepository fileReadRepository)
        {
            _fileReadRepository = fileReadRepository;
        }

        public async Task<IEnumerable<BaseFileDto>> GetAllImageOfIdAsync(long id, int? width, int? height)
        {
            return await _fileReadRepository.GetAllImageOfIdAsync(id, width, height);
        }

        public async Task<BaseFileDto> GetFileByIdQuery(long id)
        {
            return await _fileReadRepository.GetFileByIdQuery(id);
        }

        public async Task<IList<BaseFileDto>> GetResizedFileByIdQueryAsync(IEnumerable<long> ids)
        {
            return await _fileReadRepository.GetResizedFileByIdQueryAsync(ids);
        }
    }
}
