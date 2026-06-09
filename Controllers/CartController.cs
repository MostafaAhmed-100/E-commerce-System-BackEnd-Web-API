using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Services.CartService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController , Authorize(Roles = AppRoles.Buyer)]
    [EnableRateLimiting("UserActivityPolicy")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("My-Cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var BuyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var Result = await _cartService.GetCartBybuyerId(BuyerId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPost("Add-Item")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto requestDto)
        {
            var BuyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var Result = await _cartService.AddToCart(BuyerId, requestDto);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpPut("Update-Quantity/{variantId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int variantId, [FromBody] UpdateCartItemQuantityRequestDto requestDto)
        {
            var BuyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var Result = await _cartService.UpdateItemQuantity( BuyerId ,variantId , requestDto);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpDelete("Remove-Item/{variantId}")]
        public async Task<IActionResult> RemoveItem([FromRoute] int variantId)
        {
            var BuyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var Result = await _cartService.RemoveFromCart(BuyerId, variantId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpDelete("Clear-Cart")]
        public async Task<IActionResult> ClearCart()
        {
            var BuyerId = Convert.ToInt32(User.FindFirstValue("ProfileId"));

            var Result = await _cartService.ClearCart(BuyerId);

            return StatusCode(Result.StatusCode, Result);
        }
    }
}
