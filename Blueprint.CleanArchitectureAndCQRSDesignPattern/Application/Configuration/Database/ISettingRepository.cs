using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database
{
    public interface ISettingRepository : IRepository<BlueprintSetting, int>
    {
        Task<BlueprintSetting> GetByName(string settingName);
    }
}