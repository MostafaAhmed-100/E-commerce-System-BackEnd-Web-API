using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class loginAndRegisterController : ControllerBase
    {
        private readonly ILoginAndRegisterService _loginAndRegisterService;

        public loginAndRegisterController(ILoginAndRegisterService loginAndRegisterService)
        {
            _loginAndRegisterService = loginAndRegisterService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginAndRegisterDto dto)
        {
            var result = await _loginAndRegisterService.Register(dto);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAndRegisterDto dto)
        {
            var result = await _loginAndRegisterService.Login(dto.UserEmail, dto.UserPassword);
            if (result == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }

    }
}
