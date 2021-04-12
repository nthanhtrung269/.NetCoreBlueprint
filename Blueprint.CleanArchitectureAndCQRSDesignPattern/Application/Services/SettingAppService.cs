using Mapster;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Settings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
{
    public class SettingAppService : ISettingAppService
    {
        private readonly ISettingRepository _settingRepository;

        public SettingAppService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<SettingDto> GetSettingAsync(int id)
        {
            BlueprintSetting rsSetting = await _settingRepository.GetByIdAsync(id);
            return rsSetting.Adapt<SettingDto>();
        }

        public async Task<string> GetSettingValueByNameAsync(string name)
        {
            BlueprintSetting rsSetting = await _settingRepository.GetByName(name);

            if (rsSetting != null)
            {
                return rsSetting.SettingValue;
            }

            return null;
        }

        public async Task<IList<SettingDto>> GetAllAsync()
        {
            List<BlueprintSetting> rsSetting = await _settingRepository.ListAsync();
            if (rsSetting != null)
            {
                return rsSetting.Adapt<IList<SettingDto>>();
            }

            return null;
        }
    }
}