using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Authentication;
using backend.Services;
using backend.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(AuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { message = "Passwords do not match." });
            }

            var result = await _authService.RegisterUserAsync(
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.Email,
                registerDto.Password
            );
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.FullName
                }
            });
        }
    }
}