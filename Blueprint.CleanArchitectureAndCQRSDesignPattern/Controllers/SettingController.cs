using Microsoft.AspNetCore.Mvc;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Settings;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers
{
    /// <summary>
    /// Setting Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingAppService _settingAppService;

        /// <summary>
        /// The SettingController.
        /// </summary>
        /// <param name="settingAppService">The settingAppService.</param>
        public SettingController(ISettingAppService settingAppService)
        {
            _settingAppService = settingAppService;
        }

        /// <summary>
        /// Gets setting by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{SettingDto}.</returns>
        [HttpGet("by-id/{id}")]
        public async Task<SettingDto> GetSettingAsync(int id)
        {
            return await _settingAppService.GetSettingAsync(id);
        }

        /// <summary>
        /// Gets setting value by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task{string}.</returns>
        [HttpGet("value-by-name/{name}")]
        public async Task<string> GetSettingValueAsync(string name)
        {
            return await _settingAppService.GetSettingValueByNameAsync(name);
        }
    }
}
