using Microsoft.EntityFrameworkCore;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database
{
    public class SettingRepository : EfRepository<DBContext, BlueprintSetting, int>, ISettingRepository
    {
        public SettingRepository(DBContext dbContext) : base(dbContext)
        {
        }

        public Task<BlueprintSetting> GetByName(string settingName)
        {
            settingName = settingName.Trim().ToLower();
            return _dbContext.RsSetting.FirstOrDefaultAsync(e => e.SettingName.ToLower() == settingName);
        }
    }
}