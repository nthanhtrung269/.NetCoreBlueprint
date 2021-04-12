using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database
{
    public class FileReadRepository : DapperRepository, IFileReadRepository
    {
        public FileReadRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
        }

        public async Task<IList<BaseFileDto>> GetResizedFileByIdQueryAsync(IEnumerable<long> ids)
        {
            const string sql = @"SELECT Id, OriginalId, OriginalFileName, Width, Height, 
                                        CompanyId, FilePath, ThumbnailPath, Extension, FileType, 
                                        Source, CloudUrl, FileData, CreatedBy, CreatedDate, ModifiedBy, 
                                        ModifiedDate
                               FROM BlueprintFile
                               WHERE Id in @ids";
            return await QueryAsync<BaseFileDto>(sql, new { ids = ids.ToArray() });
        }

        public async Task<BaseFileDto> GetFileByIdQuery(long id)
        {
            const string sql = @"SELECT Id, OriginalId, OriginalFileName, Width, Height, 
                                        CompanyId, FilePath, ThumbnailPath, Extension, FileType, 
                                        Source, CloudUrl, FileData, CreatedBy, CreatedDate, ModifiedBy, 
                                        ModifiedDate
                               FROM BlueprintFile
                               WHERE Id = @Id";
            return await QuerySingleOrDefaultAsync<BaseFileDto>(sql, new { id });
        }

        public async Task<IEnumerable<BaseFileDto>> GetAllImageOfIdAsync(long id, int? width, int? height)
        {
            const string sql = @"SELECT Id, OriginalId, OriginalFileName, Width, Height, 
                                        CompanyId, FilePath, ThumbnailPath, Extension, FileType, 
                                        Source, CloudUrl, FileData, CreatedBy, CreatedDate, ModifiedBy, 
                                        ModifiedDate
                               FROM BlueprintFile
                               WHERE Id = @Id OR
                                        (OriginalId = @Id
                                        AND (@Height IS NULL OR Height = @Height)
                                        AND (@Width IS NULL OR Width = @Width))";

            var fileDtos = await QueryAsync<BaseFileDto>(sql,
                new
                {
                    Id = id,
                    Width = width,
                    Height = height,
                });

            return fileDtos;
        }
    }
}
