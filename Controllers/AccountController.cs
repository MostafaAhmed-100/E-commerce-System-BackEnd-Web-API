using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Services.AccountService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController , Authorize]
    [EnableRateLimiting("UserActivityPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpDelete("Delete-Account")]
        public async Task<IActionResult> DeleteAccount()
        {

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var DeleteAccount = await _accountService.DeleteAccountAsync(userId);

            return StatusCode(DeleteAccount.StatusCode, DeleteAccount);
        }
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var ChangePassword = await _accountService.ChangePasswordAsync( changePasswordRequestDto , userId);

            return StatusCode(ChangePassword.StatusCode, ChangePassword);
        }
    }
}