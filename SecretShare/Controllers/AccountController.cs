using Microsoft.AspNetCore.Mvc;
using SecretShare.Models.Domains;
using System.Threading.Tasks;
using SecretShare.Models.Service.Abstract;

namespace SecretShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.RegisterAsync(model);

            if (result.IsSuccess)
            {
                return Ok(new { Result = "User registered successfully" });
            }
            return BadRequest(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.LoginAsync(model);
            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create user: {result.Message}");
            }

            return Ok(result.Data);
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.Refresh(model);
            if (!result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create user: {result.Message}");
            }

            return Ok(result.Data);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _accountService.LogoutAsync();

            if (result.IsSuccess)
            {
                return Ok(new { Result = "User logged out successfully" });
            }
            return BadRequest(result.Message);
        }
    }
}
