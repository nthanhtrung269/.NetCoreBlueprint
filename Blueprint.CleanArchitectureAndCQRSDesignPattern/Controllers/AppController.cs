using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers
{
    /// <summary>
    /// The AppController.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly ReadmeSettings _configuration;

        /// <summary>
        /// The ProgramController constructor.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="config">The config.</param>
        public AppController(ILogger<AppController> logger,
            IOptions<ReadmeSettings> config)
        {
            _configuration = config.Value;
        }

        /// <summary>
        /// Returns Readme information.
        /// </summary>
        /// <returns>Dynamic.</returns>
        [Route("readme")]
        [HttpGet]
        public dynamic Readme()
        {
            return new
            {
                Server = $"{System.Net.Dns.GetHostName()}",
                BuildNumber = _configuration.BuildNumber,
                BuildDate = _configuration.BuildDate,
                Commit = _configuration.Commit,
                Branch = _configuration.Branch
            };
        }
    }
}
