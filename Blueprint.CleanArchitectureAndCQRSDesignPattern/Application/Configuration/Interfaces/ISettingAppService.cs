using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface ISettingAppService
    {
        Task<SettingDto> GetSettingAsync(int id);
        Task<string> GetSettingValueByNameAsync(string name);
        Task<IList<SettingDto>> GetAllAsync();
    }
}