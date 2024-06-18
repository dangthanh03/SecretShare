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

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
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
            var result = await _accountService.LoginAsync(model);

            if (result.IsSuccess)
            {
                return Ok(new { Token = result.Data }); // Give the user a token for authentication
            }
            return Unauthorized(result.Message);
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
