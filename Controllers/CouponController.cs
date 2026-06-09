using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Services.CouponService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    [EnableRateLimiting("UserActivityPolicy")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost("Create-Coupon")]
        [Authorize(Roles = AppRoles.Seller)] 
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequestDto requestDto)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));
            var Result = await _couponService.CreateCouponAsync(requestDto, sellerId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPut("Update-Coupon/{couponId}")]
        [Authorize(Roles = AppRoles.Seller)]
        public async Task<IActionResult> UpdateCoupon([FromRoute] int couponId, [FromBody] UpdateCouponRequestDto requestDto)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));
            var Result = await _couponService.UpdateCouponAsync(requestDto, couponId, sellerId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpDelete("Delete-Coupon/{couponId}")]
        [Authorize(Roles = AppRoles.Seller)] 
        public async Task<IActionResult> DeleteCoupon([FromRoute] int couponId)
        {
            var sellerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));
            var Result = await _couponService.DeleteCouponAsync(couponId, sellerId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpGet("Validate-Coupon/{couponCode}")]
        public async Task<IActionResult> ValidateCoupon([FromRoute] string couponCode)
        {
            var Result = await _couponService.GetCouponByCodeAsync(couponCode);

            return StatusCode(Result.StatusCode, Result);
        }
    }
}