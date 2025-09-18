using Ecommerce.Application.Dtos.Auth;
using Ecommerce.Application.services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var user = await _authService.RegisterAsync(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Username or email already exists")
                    return Conflict(new { message = ex.Message }); 
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDto dto)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid refresh token")
                    return Unauthorized(new { message = ex.Message }); // 401 Unauthorized
                if (ex.Message == "Refresh token expired")
                    return Unauthorized(new { message = ex.Message });
                if (ex.Message == "Refresh token has been revoked")
                    return Unauthorized(new { message = ex.Message });
                if (ex.Message == "User not found for this token")
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
