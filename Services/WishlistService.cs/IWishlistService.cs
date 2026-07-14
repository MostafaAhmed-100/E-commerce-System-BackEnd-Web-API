using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;

namespace WebApplication1.Services.WishlistService.cs
{
    public interface IWishlistService
    {
        Task<ApiResponseDto<string>> CreateWishlistAsync(int BuyerId, CreateWishlistRequestDto createWishlistRequest);

        Task<ApiResponseDto<IEnumerable<WishlistDto>>> GetBuyerWishlistsAsync(int BuyerId);

        Task<ApiResponseDto<string>> DeleteWishlistAsync(int wishlistId);

        Task<ApiResponseDto<string>> ToggleWishlistItemAsync(int wishlistId , int VariantId);

        Task<ApiResponseDto<PaginatedResponseDto<WishlistItemResponseDto>>> GetWishlistItemsAsync(int WishlistId , PaginationRequestDto paginationRequest);
    }
}
