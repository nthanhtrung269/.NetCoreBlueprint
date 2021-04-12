using Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.DataServices
{
    public interface ISettingRepository : IGenericRepository<Setting, int>
    {
        Setting GetByName(string settingName);
    }
}
