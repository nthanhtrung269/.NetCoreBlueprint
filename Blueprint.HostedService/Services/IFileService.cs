using Blueprint.HostedService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.HostedService.Services
{
    public interface IFileService
    {
        Task<IList<BaseFileDto>> GetResizedFileByIdQueryAsync(IEnumerable<long> ids);
        Task<BaseFileDto> GetFileByIdQuery(long id);
        Task<IEnumerable<BaseFileDto>> GetAllImageOfIdAsync(long id, int? width, int? height);
    }
}
