using Blueprint.CustomJwtMiddleware.Helpers;
using Blueprint.CustomJwtMiddleware.Models;
using Blueprint.CustomJwtMiddleware.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blueprint.CustomJwtMiddleware.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            _logger.LogInformation($"Call to {nameof(Authenticate)}");
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation($"Call to {nameof(GetAll)}");
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
