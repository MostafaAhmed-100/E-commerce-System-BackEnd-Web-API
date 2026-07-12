using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.Json;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Services.AccountService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
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

            var ChangePassword = await _accountService.ChangePasswordAsync(changePasswordRequestDto, userId);

            return StatusCode(ChangePassword.StatusCode, ChangePassword);
        }
        [Authorize]
        [HttpGet("GetMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("Role");
            var profileIdString = User.FindFirstValue("ProfileId");
            if (string.IsNullOrEmpty(role) || !int.TryParse(profileIdString, out int profileId))
            {
                return Unauthorized(new { isSuccess = false, message = "Invalid token claims." });
            }
            if (role == AppRoles.Buyer)
            {
                var result = await _accountService.GetBuyerProfileAsync(profileId);
                return StatusCode(result.StatusCode, result);
            }
            else if (role == AppRoles.Seller)
            {
                var result = await _accountService.GetSellerProfileAsync(profileId);
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(403, new { isSuccess = false, message = "Profile access is restricted to Buyers and Sellers." });
        }

        [Authorize(Roles = "Buyer")]
        [HttpPut("Buyer/Profile")]
        public async Task<IActionResult> UpdateBuyerProfile([FromBody] UpdateBuyerProfileRequestDto updateBuyerProfile)
        {
            var profileId = Convert.ToInt32(User.FindFirstValue("ProfileId"));
            var result = await _accountService.UpdateBuyerProfileAsync(profileId, updateBuyerProfile);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("Seller/Profile")]
        public async Task<IActionResult> UpdateSellerProfile([FromBody] UpdateSellerProfileRequestDto updateSellerProfile)
        {
            var profileId = Convert.ToInt32(User.FindFirstValue("ProfileId"));
            var result = await _accountService.UpdateSellerProfileAsync(profileId, updateSellerProfile);
            return StatusCode(result.StatusCode, result);
        }
    }
}
