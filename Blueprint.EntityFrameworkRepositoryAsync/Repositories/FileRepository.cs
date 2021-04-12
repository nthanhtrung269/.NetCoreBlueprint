using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.EntityFrameworkRepositoryAsync
{
    public class FileRepository : EfRepository<DBContext, BlueprintFile, long>, IFileRepository
    {
        public FileRepository(DBContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> DeleteFiles(List<BlueprintFile> files)
        {
            _dbContext.BlueprintFile.RemoveRange(files);
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
            List<BlueprintFile> rsFiles = _dbContext.BlueprintFile
                .Where(f => supportFiles.Contains(f.Extension)
                            && supportedCategoryTypes.Contains(f.FileType)
                            && f.OriginalId == null
                            && (f.BackgroudProcessingStatus == null || f.BackgroudProcessingStatus == 0))
                .OrderByDescending(f => f.Id)
                .Take(numberItems).ToList();

            if (rsFiles == null || !rsFiles.Any())
            {
                rsFiles = _dbContext.BlueprintFile
                .Where(f => supportFiles.Contains(f.Extension)
                            && supportedCategoryTypes.Contains(f.FileType)
                            && f.OriginalId == null
                            && f.BackgroudProcessingStatus == 1
                            && f.CreatedDate != null
                            && f.CreatedDate.Value.AddMinutes(10) <= DateTime.UtcNow)
                .OrderByDescending(f => f.Id)
                .Take(numberItems).ToList();
            }

            foreach (var rsFile in rsFiles)
            {
                rsFile.BackgroudProcessingStatus = 2;
            }

            _dbContext.UpdateRange(rsFiles);
            _dbContext.SaveChanges();

            return rsFiles;
        }

        public void UpdateRangeForBackgroudProcessing(List<BlueprintFile> rsFiles, int backgroudProcessingStatus)
        {
            foreach (var rsFile in rsFiles)
            {
                rsFile.BackgroudProcessingStatus = backgroudProcessingStatus;
            }

            _dbContext.UpdateRange(rsFiles);
            _dbContext.SaveChanges();
        }
    }
}
