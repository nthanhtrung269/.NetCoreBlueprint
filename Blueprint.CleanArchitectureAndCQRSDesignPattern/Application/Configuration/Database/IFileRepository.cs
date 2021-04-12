using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database
{
    public interface IFileRepository : IRepository<BlueprintFile, long>
    {
        Task<bool> DeleteFiles(List<BlueprintFile> files);
        IEnumerable<BlueprintFile> GetFiles(long[] ids, int? width, int? height);
        IEnumerable<BlueprintFile> GetFiles(long id, int? width, int? height);
        IEnumerable<BlueprintFile> GetFilesForBackgroudProcessing(List<string> supportFiles, List<string> supportedCategoryTypes, int numberItems);
        void UpdateRangeForBackgroudProcessing(List<BlueprintFile> rsFiles, int backgroudProcessingStatus);
    }
}