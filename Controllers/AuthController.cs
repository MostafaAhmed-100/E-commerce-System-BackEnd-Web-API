using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            var Result = await _authService.LoginAsync(requestDto);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
        {
            var Result = await _authService.RegisterAsync(requestDto);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost("Register-Seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterSellerRequestDto requestDto)
        {
            var Result = await _authService.RegisterSellerAsync(requestDto);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost("Register-Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequestDto requestDto)
        {
            var Result = await _authService.RegisterAdminAsync(requestDto);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var Result = await _authService.RefreshTokenAsync(requestDto);
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpGet("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string code)
        {
            var Result = await _authService.ConfirmEmailAsync(userId, code);
            return StatusCode(Result.StatusCode, Result);
        }
    }
}