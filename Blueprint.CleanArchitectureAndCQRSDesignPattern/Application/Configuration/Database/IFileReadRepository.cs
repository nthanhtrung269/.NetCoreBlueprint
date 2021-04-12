using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database
{
    public interface IFileReadRepository : IReadRepository
    {
        Task<IList<BaseFileDto>> GetResizedFileByIdQueryAsync(IEnumerable<long> ids);
        Task<BaseFileDto> GetFileByIdQuery(long id);
        Task<IEnumerable<BaseFileDto>> GetAllImageOfIdAsync(long id, int? width, int? height);
    }
}
