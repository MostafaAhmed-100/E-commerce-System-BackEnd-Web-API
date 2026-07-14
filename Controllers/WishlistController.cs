using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.Services.WishlistService;
using WebApplication1.Services.WishlistService.cs;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetBuyerId()
        {
            var buyerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(buyerIdClaim, out int buyerId))
            {
                return buyerId;
            }

            throw new UnauthorizedAccessException("Invalid token or Buyer ID not found.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateWishlist([FromBody] CreateWishlistRequestDto dto)
        {
            int buyerId = GetBuyerId();
            var result = await _wishlistService.CreateWishlistAsync(buyerId, dto);
            return StatusCode(result.StatusCode ,result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBuyerWishlists()
        {
            int buyerId = GetBuyerId();
            var result = await _wishlistService.GetBuyerWishlistsAsync(buyerId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> DeleteWishlist(int wishlistId)
        {
            var result = await _wishlistService.DeleteWishlistAsync(wishlistId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{wishlistId}/toggle-item")]
        public async Task<IActionResult> ToggleWishlistItem(int wishlistId, [FromBody] AddWishlistItemDto dto)
        {
            var result = await _wishlistService.ToggleWishlistItemAsync(wishlistId, dto.ProductId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{wishlistId}/items")]
        public async Task<IActionResult> GetWishlistItems(int wishlistId, [FromQuery] PaginationRequestDto paginationRequest)
        {
            var result = await _wishlistService.GetWishlistItemsAsync(wishlistId, paginationRequest);
            return StatusCode(result.StatusCode, result);
        }
    }
}