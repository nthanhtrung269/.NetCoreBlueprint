using Blueprint.CustomExceptionHandlerMiddleware.Project.DataServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices
{
    public class CachingWorkerService : ICachingWorkerService
    {
        private readonly ILogger<CachingWorkerService> _logger;
        private readonly ISettingRepository _settingRepository;
        private readonly IMemoryCache _cache;

        public CachingWorkerService(ILogger<CachingWorkerService> logger,
            ISettingRepository settingRepository,
            IMemoryCache cache)
        {
            _logger = logger;
            _settingRepository = settingRepository;
            _cache = cache;
        }

        /// <summary>
        /// Is logging database.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool IsLoggingDatabase()
        {
            throw new System.NotImplementedException();
        }

        public string GetAuthorizeToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
