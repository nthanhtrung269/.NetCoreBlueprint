using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database
{
    public class FileRepository : EfRepository<DBContext, BlueprintFile, long>, IFileRepository
    {
        public FileRepository(DBContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> DeleteFiles(List<BlueprintFile> blueprintFiles)
        {
            _dbContext.BlueprintFile.RemoveRange(blueprintFiles);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public IEnumerable<BlueprintFile> GetFiles(long[] ids, int? width, int? height)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlueprintFile> GetFiles(long id, int? width, int? height)
        {
            return _dbContext.BlueprintFile.Where(file => (file.Id == id)
                || ((width == null || file.Width == width.Value)
                    && (height == null || file.Height == height.Value)
                    && (file.OriginalId == id))).ToList();
        }

        public IEnumerable<BlueprintFile> GetFilesForBackgroudProcessing(List<string> supportFiles, List<string> supportedCategoryTypes, int numberItems)
        {
            List<BlueprintFile> blueprintFiles = _dbContext.BlueprintFile
                .Where(f => supportFiles.Contains(f.Extension)
                            && supportedCategoryTypes.Contains(f.FileType)
                            && f.OriginalId == null
                            && (f.BackgroudProcessingStatus == null || f.BackgroudProcessingStatus == 0))
                .OrderByDescending(f => f.Id)
                .Take(numberItems).ToList();

            if (blueprintFiles == null || !blueprintFiles.Any())
            {
                blueprintFiles = _dbContext.BlueprintFile
                .Where(f => supportFiles.Contains(f.Extension)
                            && supportedCategoryTypes.Contains(f.FileType)
                            && f.OriginalId == null
                            && f.BackgroudProcessingStatus == 1
                            && f.CreatedDate != null
                            && f.CreatedDate.Value.AddMinutes(10) <= DateTime.UtcNow)
                .OrderByDescending(f => f.Id)
                .Take(numberItems).ToList();
            }

            foreach (var blueprintFile in blueprintFiles)
            {
                blueprintFile.BackgroudProcessingStatus = (int)BackgroudProcessingStatus.IsProcessing;
            }

            _dbContext.UpdateRange(blueprintFiles);
            _dbContext.SaveChanges();

            return blueprintFiles;
        }

        public void UpdateRangeForBackgroudProcessing(List<BlueprintFile> blueprintFiles, int backgroudProcessingStatus)
        {
            foreach (var blueprintFile in blueprintFiles)
            {
                blueprintFile.BackgroudProcessingStatus = backgroudProcessingStatus;
            }

            _dbContext.UpdateRange(blueprintFiles);
            _dbContext.SaveChanges();
        }
    }
}