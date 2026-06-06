using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Services.AccountService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController , Authorize]
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
    }
}