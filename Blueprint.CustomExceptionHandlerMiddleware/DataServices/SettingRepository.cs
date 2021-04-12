using Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels;
using Blueprint.CustomExceptionHandlerMiddleware.Project.DataServices;
using System;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project
{
    public class SettingRepository : GenericRepository<BlueprintContext, Setting, int>, ISettingRepository
    {
        public SettingRepository(BlueprintContext dbContext) : base(dbContext)
        {
        }

        public Setting GetByName(string settingName)
        {
            throw new NotImplementedException();
        }
    }
}
