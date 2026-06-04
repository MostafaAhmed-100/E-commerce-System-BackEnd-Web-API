using WebApplication1.DTOS;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication1.Services.CartService
{
    public interface ICartService
    {
        public Task<ApiResponseDto<CartResponseDto>> GetCartBybuyerId(int buyerId);
        public Task<ApiResponseDto<CartResponseDto>?> AddToCart(int buyerId, AddToCartRequestDto addToCartRequestDto);
        public Task<ApiResponseDto<CartResponseDto>> UpdateItemQuantity(int buyerId , int variantId,  UpdateCartItemQuantityRequestDto UpdateCartItemQuantityRequestDto);
        public Task<ApiResponseDto<CartResponseDto>?> RemoveFromCart(int buyerId, int variantId);
        public Task<ApiResponseDto<CartResponseDto>?> ClearCart(int buyerId );
    }
}
